using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json.Linq;

namespace Unit.Suite;

public abstract class
    StateOrScopePropertyTests<TFormatter, TFormatterOptions> : FormatterTestsBase<TFormatter,
    TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected StateOrScopePropertyTests(SpecBase<TFormatterOptions> spec, ITestOutputHelper testOutputHelper) :
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

    [Theory]
    [CombinatorialData]
    public void DuplicatePropertyNames_NeverLoggedTwice(PropertyNameDuplicateHandling duplicateHandling)
    {
        // Arrange
        var logger = LoggerBuilder
            .AddStaticScope(null, "Key", "FromScope1")
            .AddStaticScope(null, "Key", "FromScope2")
            .With(ConfigActions.IncludeScopes)
            .With(Spec.ConfigurePropertyNameDuplicateHandling(duplicateHandling))
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
    public virtual void DuplicatePropertyNames_NeverOverwritesStandardProperties(
        PropertyNameDuplicateHandling duplicateHandling)
    {
        // Arrange
        var logger = LoggerBuilder
            .AddStaticScope(null, "Timestamp", "overwritten")
            .AddStaticScope(null, "LogLevel", "overwritten")
            .AddStaticScope(null, "Message", "overwritten")
            .AddStaticScope(null, "Exception", "overwritten")
            .AddStaticScope(null, "Category", "overwritten")
            .AddStaticScope(null, "EventId", "overwritten")
            .With(ConfigActions.IncludeScopes)
            .With(Spec.ConfigurePropertyNameDuplicateHandling(duplicateHandling))
            .With(Spec.ConfigureIncludeEventHandling(true))
            .Build();
        var exception = new Exception("Test exception");

        // Act
        logger.LogInformation(42, exception,
            "from message: {Timestamp} {LogLevel} {Message} {Exception} {Category} {EventId}",
            "overwritten", "overwritten", "overwritten", "overwritten", "overwritten", "overwritten");

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        var jtoken = JToken.Parse(message);
        // issues with FluentAssertions.Json and all non-string values
        // jtoken.Should().HaveElementSomewhere(Spec.ElementNameTimestamp).Which.Should().HaveValue(C.ExpectedDefaultTimestampStringLocal);
        if (Spec.SupportsTimeProvider)
            message.Should().Contain($"\"{Spec.ElementNameTimestamp}\":\"{C.ExpectedDefaultTimestampStringLocal}\"");
        else
            message.Should().NotContain($"\"{Spec.ElementNameTimestamp}\":\"overwritten\"");
        jtoken.Should().HaveElementSomewhere(Spec.ElementNameLogLevel).Which.Should()
            .HaveValue(Spec.LogLevelStrings[LogLevel.Information]);
        jtoken.Should().HaveElementSomewhere(Spec.ElementNameMessage).Which.Should()
            .HaveValue("from message: overwritten overwritten overwritten overwritten overwritten overwritten");
        var actualException = jtoken.Should().HaveElementSomewhere(Spec.ElementNameException).Which.Value<string>();
        actualException.Should().Contain(exception.GetType().FullName!);
        actualException.Should().Contain(exception.Message);
        jtoken.Should().HaveElementSomewhere(Spec.ElementNameCategory).Which.Should().HaveValue(C.DefaultLoggerName);
        jtoken.Should().HaveElementSomewhere(Spec.ElementNameEventId).Which.Should().HaveValue("42");
    }
}
