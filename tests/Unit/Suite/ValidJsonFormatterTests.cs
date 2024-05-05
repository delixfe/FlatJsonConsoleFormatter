using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json.Linq;

namespace Unit.Suite;

public abstract class
    ValidJsonFormatterTests<TFormatter, TFormatterOptions, TSpec> : FormatterTestsBase<TFormatter,
    TFormatterOptions, TSpec>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
    where TSpec : SpecBase<TFormatterOptions>
{
    protected ValidJsonFormatterTests(TSpec spec, ITestOutputHelper testOutputHelper) : base(
        spec, testOutputHelper)
    {
    }

    [Fact]
    public void HelloWorld()
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.LogInformation("Hello, world!");

        // Assert
        logger.Formatted.Should().Contain("Hello, world!");
        logger.Formatted.Should().BeValidJson();
    }

    [Theory]
    [InlineData("Hello, world!")]
    [InlineData("{Message}")]
    public void SimpleStringMessage_ValidJson(string message)
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.LogInformation(message);

        // Assert
        logger.Formatted.Should().BeValidJson();
    }


    [Fact]
    public void Log_UsesCorrectStandardElementNames()
    {
        // Arrange
        var logger = LoggerBuilder.With(Spec.ConfigureIncludeEventHandling(true)).Build();
        var ex = new Exception("Test exception");

        // Act
        logger.LogInformation(new EventId(41, "FortyTwo"), ex, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();

        using var _ = new AssertionScope();

        var log = JToken.Parse(logger.Formatted!);

        log.Should().HaveElement(Spec.ElementNameLogLevel, "LogLevel should be named {0}", Spec.ElementNameLogLevel);
        log.Should().HaveElement(Spec.ElementNameMessage, "Message should be named {0}", Spec.ElementNameMessage);
        log.Should().HaveElement(Spec.ElementNameException, "Exception should be named {0}", Spec.ElementNameException);
        log.Should().HaveElement(Spec.ElementNameCategory, "Category should be named {0}", Spec.ElementNameCategory);
        log.Should().HaveElement(Spec.ElementNameEventId, "EventId should be named {0}", Spec.ElementNameEventId);
    }

    [Theory]
    [CombinatorialData]
    public void Log_CorrectLogLevel(LogLevel level)
    {
        if (level == LogLevel.None) return;

        // Arrange
        var logger = LoggerBuilder.Build();
        var expected = Spec.LogLevelStrings[level];

        // Act
        logger.Log(level, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        JToken.Parse(logger.Formatted!) //
            .Should().HaveElement(Spec.ElementNameLogLevel).Which //
            .Should().BeOfType<JValue>().Which.Value.Should().Be(expected);
    }
}
