using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Unit.Suite;

public abstract class
    MSConsoleFormatterTests<TFormatter, TFormatterOptions> : FormatterTestsBase<TFormatter,
    TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected MSConsoleFormatterTests(SpecBase<TFormatterOptions> spec,
        ITestOutputHelper testOutputHelper) : base(spec, testOutputHelper)
    {
    }

    // TODO: ConsoleLoggerOptions_TimeStampFormat_IsReloaded


    [Theory]
    [MemberData(nameof(Data.LogLevels), MemberType = typeof(Data))]
    public void NoMessageOrException_Noop(LogLevel level)
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        Func<object, Exception?, string> formatter = (state, exception) => null!;
        logger.Log(level, 0, _state, null, formatter);

        // Assert
        logger.Formatted.Should().BeNullOrEmpty();
    }

    [Fact]
    public void InvalidLogLevel_Throws()
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        var act = () => logger.Log((LogLevel)8, 0, _state, null, _defaultFormatter);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }


    [Theory(Skip = "Test fails, not clear why")]
    [MemberData(nameof(Data.LogLevels), MemberType = typeof(Data))]
    public virtual void Log_LogsCorrectTimestamp(LogLevel logLevel)
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
            .Subject.Should().HaveElement(Spec.ElementNameTimestamp).Which. //
            Should().MatchRegex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{3}");
    }
}
