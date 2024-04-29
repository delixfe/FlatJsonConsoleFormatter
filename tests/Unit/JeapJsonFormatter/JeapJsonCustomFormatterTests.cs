using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions.Execution;
using JsonConsoleFormatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Unit.JeapJsonFormatter;

public class
    JeapJsonCustomFormatterTests : FormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonCustomFormatterTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }

    [Theory]
    [CombinatorialData]
    public void Log_IgnoresTimestampFormatAndRespectsUseUtc(bool useUtcTimeStamp, bool unsafeRelaxedJson,
        [CombinatorialValues(null, "O", "hh:mm:ss")]
        string? timestampFormat)

    {
        // Arrange
        var logger = LoggerBuilder
            .With(o =>
            {
                o.TimestampFormat = timestampFormat;
                o.UseUtcTimestamp = useUtcTimeStamp;
                o.JsonWriterOptions = new JsonWriterOptions
                {
                    // otherwise escapes for timezone formatting from + to \u002b
                    Encoder = unsafeRelaxedJson
                        ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        : JavaScriptEncoder.Default, //  
                    Indented = false
                };
            })
            .Build();

        var expected = $"""
                        "{Spec.ElementNameTimestamp}":"{(useUtcTimeStamp ? C.ExpectedDefaultTimestampStringUtc : C.ExpectedDefaultTimestampStringLocal)}"
                        """;

        // Act
        logger.LogInformation("Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        logger.Formatted.Should().Contain(expected);
    }

    [Fact]
    public void Log_UsesCorrectStandardElementNames()
    {
        // Arrange
        var logger = LoggerBuilder.With(o => o.IncludeEventId = true).Build();
        var ex = new Exception("Test exception");

        // Act
        logger.LogInformation(new EventId(41, "FortyTwo"), ex, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();

        using var _ = new AssertionScope();

        var log = JToken.Parse(logger.Formatted!);

        log.Should().HaveElement(Spec.ElementNameLogLevel);
        log.Should().HaveElement(Spec.ElementNameMessage);
        log.Should().HaveElement(Spec.ElementNameException);
        log.Should().HaveElement(Spec.ElementNameCategory);
        log.Should().HaveElement(Spec.ElementNameEventId);
        log.Should().HaveElement(Spec.ElementNameEventName);
    }

    public TheoryData<LogLevel, string> LogLevelWithExpectations()
    {
        var data = new TheoryData<LogLevel, string>();
        foreach (var (level, expected) in Spec.LogLevelStrings)
        {
            data.Add(level, expected);
        }

        return data;
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

    [Fact]
    public void Log_IncludeThreadName_OutputsThreadName()
    {
        // Arrange
        var logger = LoggerBuilder.With(o => o.IncludeThreadName = true).Build();
        var expectedName = Thread.CurrentThread.Name;

        // Act
        logger.LogInformation("Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        JToken.Parse(logger.Formatted!) //
            .Should().HaveElement(Spec.ElementNameThreadName).Which //
            .Should().BeOfType<JValue>().Which.Value.Should().Be(expectedName);
    }
}
