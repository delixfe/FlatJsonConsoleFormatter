using System.Collections.Frozen;
using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Json;
using JsonConsoleFormatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Unit.Infrastructure;
using Xunit.Abstractions;

namespace Unit;

public class JeapJsonFormatterTests : FormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    protected readonly Spec _spec = new();

    public JeapJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(FakeLoggerBuilder.JeapJson(),
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
                        "{_spec.ElementNameTimestamp}":"{(useUtcTimeStamp ? C.ExpectedDefaultTimestampStringUtc : C.ExpectedDefaultTimestampStringLocal)}"
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

        log.Should().HaveElement(_spec.ElementNameLogLevel);
        log.Should().HaveElement(_spec.ElementNameMessage);
        log.Should().HaveElement(_spec.ElementNameException);
        log.Should().HaveElement(_spec.ElementNameCategory);
        log.Should().HaveElement(_spec.ElementNameEventId);
        log.Should().HaveElement(_spec.ElementNameEventName);
    }

    public TheoryData<LogLevel, string> LogLevelWithExpectations()
    {
        var data = new TheoryData<LogLevel, string>();
        foreach (var (level, expected) in _spec.LogLevelStrings)
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
        var expected = _spec.LogLevelStrings[level];

        // Act
        logger.Log(level, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        JToken.Parse(logger.Formatted!) //
            .Should().HaveElement(_spec.ElementNameLogLevel).Which //
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
            .Should().HaveElement(_spec.ElementNameThreadName).Which //
            .Should().BeOfType<JValue>().Which.Value.Should().Be(expectedName);
    }

    public class Spec
    {
        public string ElementNameTimestamp { get; } = "@timestamp";
        public string ElementNameLogLevel { get; } = "level";
        public string ElementNameMessage { get; } = "message";
        public string ElementNameException { get; } = "exception";
        public string ElementNameCategory { get; } = "logger";
        public string ElementNameEventId { get; } = "eventId";

        public string ElementNameEventName { get; } = "eventName";

        public string ElementNameThreadName { get; } = "thread_name";

        public IReadOnlyDictionary<LogLevel, string> LogLevelStrings { get; } = new Dictionary<LogLevel, string>
        {
            // TODO: check against otel
            [LogLevel.Trace] = "TRACE",
            [LogLevel.Debug] = "DEBUG",
            [LogLevel.Information] = "INFO",
            [LogLevel.Warning] = "WARN",
            [LogLevel.Error] = "ERROR",
            [LogLevel.Critical] = "CRITIC"
        }.ToFrozenDictionary();
    }
}
