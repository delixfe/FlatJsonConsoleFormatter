using System.Collections.Frozen;
using JsonConsoleFormatters;
using Microsoft.Extensions.Logging;

namespace Unit.JeapJsonFormatter;

public class JeapJsonFormatterSpec : SpecBase<JeapJsonConsoleFormatterOptions>
{
    public override string ElementNameTimestamp { get; } = "@timestamp";
    public override string ElementNameLogLevel { get; } = "level";
    public override string ElementNameMessage { get; } = "message";
    public override string ElementNameException { get; } = "exception";
    public override string ElementNameCategory { get; } = "logger";
    public override string ElementNameEventId { get; } = "eventId";

    public override string ElementNameEventName { get; } = "eventName";

    public override string ElementNameThreadName { get; } = "thread_name";

    public override bool OutputsOriginalFormat { get; } = false;
    public override bool ScopeOutputsMessage { get; } = false;
    public override bool ScopeOutputsOriginalFormat { get; } = false;


    public override IReadOnlyDictionary<LogLevel, string> LogLevelStrings { get; } = new Dictionary<LogLevel, string>
    {
        // TODO: check against otel
        [LogLevel.Trace] = "TRACE",
        [LogLevel.Debug] = "DEBUG",
        [LogLevel.Information] = "INFO",
        [LogLevel.Warning] = "WARN",
        [LogLevel.Error] = "ERROR",
        [LogLevel.Critical] = "CRITIC"
    }.ToFrozenDictionary();

    public override FakeLoggerBuilder<JeapJsonConsoleFormatterOptions> CreateLoggerBuilder() => new(
        (optionsMonitor, timeProvider) => new JeapJsonConsoleFormatter(optionsMonitor, timeProvider),
        [ConfigActions.DontIncludeScopes]);
}
