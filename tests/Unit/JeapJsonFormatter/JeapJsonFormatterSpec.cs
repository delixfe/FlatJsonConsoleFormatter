using System.Collections.Frozen;
using System.Text.Json;
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

    // additional attributes

    public string ElementNameEventName { get; } = "eventName";
    public string ElementNameThreadName { get; } = "thread_name";
    public string ElementNameSeverity { get; } = "severity";

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
        [LogLevel.Critical] = "FATAL"
    }.ToFrozenDictionary();

    public override bool SupportsTimeProvider { get; } = true;

    public override string MapStateOrScopeElementNames(string name) => JsonNamingPolicy.CamelCase.ConvertName(name);

    public override FakeLoggerBuilder<JeapJsonConsoleFormatterOptions> CreateLoggerBuilder() => new(
        (optionsMonitor, timeProvider) => new JeapJsonConsoleFormatter(optionsMonitor, timeProvider),
        Array.Empty<Action<JeapJsonConsoleFormatterOptions>>());

    public override Action<JeapJsonConsoleFormatterOptions> ConfigureIncludeEventHandling(bool includeEventHandling) =>
        o =>
        {
            if (!includeEventHandling)
                throw new NotSupportedException("JeapJsonConsoleFormatter does not support disabling IncludeEventId");
        };
}
