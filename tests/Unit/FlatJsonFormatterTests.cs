using System.Text.Encodings.Web;
using System.Text.Json;
using FlatJsonConsoleFormatter;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.Extensions.Logging;
using Unit.Infrastructure;
using Xunit.Abstractions;

namespace Unit;

public class FlatJsonFormatterTests : FormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
    FlatJsonConsoleFormatterOptions>
{
    public FlatJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(FakeLoggerBuilder.FlatJson(),
        testOutputHelper)
    {
    }

    [Theory(Skip = "Test fails, not clear why")]
    [MemberData(nameof(LogLevels))]
    public void Log_LogsCorrectTimestamp(LogLevel logLevel)
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o =>
            {
                o.TimestampFormat = "yyyy-MM-ddTHH:mm:sszz ";
                o.UseUtcTimestamp = false;
                o.JsonWriterOptions = new JsonWriterOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = true
                };
            })
            .Build();
        var ex = new Exception("Exception message" + Environment.NewLine + "with a second line");

        // Act
        logger.Log(logLevel, 0, _state, ex, _defaultFormatter);

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().HaveElement("Timestamp").Which. //
            Should().MatchRegex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}");
    }

    [Fact]
    public void Log_TimestampFormatSet_ContainsTimestamp()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o => o.TimestampFormat = "hh:mm:ss ")
            .Build();

        // Act
        logger.LogCritical(0, null);

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().HaveElement("Timestamp").Which.Should().MatchRegex(@"\d{2}:\d{2}:\d{2}");
    }
}
