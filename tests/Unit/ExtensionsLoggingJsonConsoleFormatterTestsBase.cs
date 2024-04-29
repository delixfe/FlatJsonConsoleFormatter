using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class
    ExtensionsLoggingJsonConsoleFormatterTestsBase<TFormatter, TFormatterOptions> : FormatterTestsBase<TFormatter,
    TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected const string _loggerName = C.DefaultLoggerName;
    // protected const string _state = "This is a test, and {curly braces} are just fine!"; 

    protected ExtensionsLoggingJsonConsoleFormatterTestsBase(SpecBase<TFormatterOptions> spec,
        ITestOutputHelper testOutputHelper) : base(spec, testOutputHelper)
    {
    }

    // TODO: Log_TimestampFormatSet_ContainsTimestamp

    // TODO: Log_NullMessage_LogsWhenMessageIsNotProvided

    // TODO: Log_ExceptionWithMessage_ExtractsInfo

    // TODO: Log_IncludeScopes_ContainsDuplicateNamedPropertiesInScope_AcceptableJson

    // TODO: Log_StateAndScopeAreCollections_IncludesMessageAndCollectionValues

    // TODO: Log_StateAndScopeContainsSpecialCaseValue_SerializesValueAsExpected

    // TODO: Log_StateAndScopeContainsFloatingPointType_SerializesValue

    // TODO: Log_StateAndScopeContainsNullValue_SerializesNull

    // TODO: Log_ScopeIsIEnumerable_SerializesKeyValuePair

    // TODO: ShouldContainInnerException

    // TODO: ShouldContainAggregateExceptions


    // TODO: remove this test
    [Fact(Skip = "Test name is a misnomer, as there is scope")]
    public void NoLogScope_DoesNotWriteAnyScopeContentToOutput_Json()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o =>
            {
                o.IncludeScopes = true;
                o.JsonWriterOptions = new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            })
            .Build();

        // Act
        using (logger.BeginScope("Scope with named parameter {namedParameter}", 123))
        using (logger.BeginScope("SimpleScope"))
            logger.Log(LogLevel.Warning, 0, "Message with {args}", 73);

        // Assert
        Assert.Contains("Message with {args}", logger.Formatted);
        Assert.Contains("73", logger.Formatted);
        Assert.Contains("{OriginalFormat}", logger.Formatted);
        Assert.Contains("namedParameter", logger.Formatted);
        Assert.Contains("123", logger.Formatted);
        Assert.Contains("SimpleScope", logger.Formatted);
    }


    [Fact]
    public virtual void Log_TimestampFormatSet_ContainsTimestamp()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o => o.TimestampFormat = "hh:mm:ss ")
            .Build();

        // Act
        logger.LogCritical(0, null);

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().HaveElement(Spec.ElementNameTimestamp).Which.Should().MatchRegex(@"\d{2}:\d{2}:\d{2}");
    }
}
