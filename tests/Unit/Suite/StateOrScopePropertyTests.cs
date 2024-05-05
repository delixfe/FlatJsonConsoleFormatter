using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json.Linq;

namespace Unit.Suite;

public abstract class
    StateOrScopePropertyTests<TFormatter, TFormatterOptions, TSpec> : FormatterTestsBase<TFormatter,
    TFormatterOptions, TSpec>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
    where TSpec : SpecBase<TFormatterOptions>
{
    public enum PropertyNameDuplication
    {
        Scope,
        Message,
        Both
    }

    protected StateOrScopePropertyTests(TSpec spec, ITestOutputHelper testOutputHelper) :
        base(spec,
            testOutputHelper)
    {
    }

    [Fact]
    public void EnabledScope_ContainsScopeProperties()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o => o.IncludeScopes = true)
            .AddStaticScope(null, "Key", "Value") //
            .Build();

        // Act
        logger.LogInformation("Hello, world!");

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().HaveElement(MappedKey("Key")).Which.Should().HaveValue("Value");
    }

    [Fact]
    public void ScopeNotEnabled_DoesNotContainScopeProperties()
    {
        // Arrange
        var logger = LoggerBuilder
            .AddStaticScope(null, "Key", "Value")
            .Build();

        // Act
        logger.LogInformation("Hello, world!");

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().NotHaveElement(MappedKey("Key"));
    }

    [Fact]
    public void DuplicatePropertyNames_NeverLoggedTwice()
    {
        // Arrange
        var logger = LoggerBuilder
            .AddStaticScope(null, "Key", "FromScope1")
            .AddStaticScope(null, "Key", "FromScope2")
            .With(ConfigActions.IncludeScopes)
            .Build();

        // Act
        // logger.LogInformation($"Key is {{Key}} {Spec.ElementNameMessage}", "FromMessage", "overwritten");
        logger.LogInformation("Key is {Key} {Message}", "FromMessage", "overwritten");

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        message.Should().Contain($"\"{MappedKey("Key")}\":", Exactly.Once());
        message.Should().Contain($"\"{Spec.ElementNameMessage}\":", Exactly.Once());
    }

    [Theory]
    [CombinatorialData]
    public void Reserved_Timestamp(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameTimestamp, Spec.ElementNameTimestamp, duplication);

    [Theory]
    [CombinatorialData]
    public void Reserved_LogLevel(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameLogLevel, Spec.ElementNameLogLevel, duplication);

    [Theory]
    [CombinatorialData]
    public void Reserved_Message(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameMessage, Spec.ElementNameMessage, duplication);

    [Theory]
    [CombinatorialData]
    public void Reserved_Exception(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameException, Spec.ElementNameException, duplication, true);

    [Theory]
    [CombinatorialData]
    public void Reserved_Category(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameCategory, Spec.ElementNameCategory, duplication);


    [Theory]
    [CombinatorialData]
    public void Reserved_EventId(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten(Spec.ElementNameEventId, Spec.ElementNameEventId, duplication, addEventId: true);


    protected virtual void PropertyIsNotOverwritten(
        string key, string elementName, PropertyNameDuplication duplication,
        bool addException = false,
        bool addEventId = false,
        Action<FakeLoggerBuilder<TFormatterOptions>>? customizeBuilder = null)
    {
        // Arrange
        var builder = LoggerBuilder
            .With(ConfigActions.IncludeScopes)
            .With(Spec.ConfigureIncludeEventHandling(true));
        customizeBuilder?.Invoke(builder);
        switch (duplication)
        {
            case PropertyNameDuplication.Scope:
            case PropertyNameDuplication.Both:
                builder.AddStaticScope(null, key, "overwrittenScope");
                break;
        }

        var logger = builder.Build();
        var exception = addException ? new Exception("Test exception") : null;
        var eventId = new EventId(42, "42");

        string messageTemplate;
        object?[] args = [];
        // Act
        switch (duplication)
        {
            case PropertyNameDuplication.Message:
            case PropertyNameDuplication.Both:
                messageTemplate = $"from message: {key}:{{{key}}}";
                args = ["overwrittenMessage"];
                break;
            case PropertyNameDuplication.Scope:
                messageTemplate = "from message";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(duplication));
        }

        if (addEventId)
        {
            logger.LogInformation(eventId, exception, messageTemplate, args);
        }
        else
        {
            logger.LogInformation(exception, messageTemplate, args);
        }


        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        var jtoken = JToken.Parse(message);
        jtoken.Should().HaveElementSomewhere(elementName).Which.Should().NotHaveValue("overwrittenScope").And
            .NotHaveValue("overwrittenMessage");

        PropertyIsNotOverwrittenAdditionalAssertions(key, elementName, duplication, message);
    }

    protected virtual void PropertyIsNotOverwrittenAdditionalAssertions(string key, string elementName,
        PropertyNameDuplication duplication, string message)
    {
        // no-op
    }
}
