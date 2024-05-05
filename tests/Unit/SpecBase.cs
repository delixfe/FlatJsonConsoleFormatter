using System.Collections.Frozen;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class SpecBase<TOptions> where TOptions : JsonConsoleFormatterOptions, new()
{
    private static readonly LogEntry<object> _logEntryAttributeNames = new();

    // official attributes
    public virtual string ElementNameTimestamp { get; } = "Timestamp";
    public virtual string ElementNameLogLevel { get; } = nameof(_logEntryAttributeNames.LogLevel);
    public virtual string ElementNameMessage { get; } = "Message";
    public virtual string ElementNameException { get; } = nameof(_logEntryAttributeNames.Exception);
    public virtual string ElementNameCategory { get; } = nameof(_logEntryAttributeNames.Category);
    public virtual string ElementNameEventId { get; } = nameof(_logEntryAttributeNames.EventId);

    public string[] OfficialElementNames => new[]
    {
        ElementNameTimestamp, ElementNameLogLevel, ElementNameMessage, ElementNameException, ElementNameCategory,
        ElementNameEventId
    };


    // additional functionality - template and message
    public virtual bool OutputsOriginalFormat { get; } = true;
    public virtual string ElementNameOriginalFormat { get; } = "{OriginalFormat}";

    // additional functionality - scope
    public virtual bool ScopeOutputsMessage { get; } = true;
    public virtual string ScopeElementNameMessage { get; } = "Message";

    public virtual bool ScopeOutputsOriginalFormat { get; } = true;
    public virtual string ScopeElementOriginalFormat { get; } = "{OriginalFormat}";

    // log levels
    public virtual IReadOnlyDictionary<LogLevel, string> LogLevelStrings { get; } = new Dictionary<LogLevel, string>
    {
        [LogLevel.Trace] = "Trace",
        [LogLevel.Debug] = "Debug",
        [LogLevel.Information] = "Information",
        [LogLevel.Warning] = "Warning",
        [LogLevel.Error] = "Error",
        [LogLevel.Critical] = "Critical"
    }.ToFrozenDictionary();

    public virtual bool SupportsTimeProvider { get; } = false;

    // additional functionality - state and scope element names
    public virtual string MapStateOrScopeElementNames(string name) => name;

    public string CreateJsonKeyValuePair(string key, JsonValue? value)
    {
        key = MapStateOrScopeElementNames(key);
        return $"\"{key}\":{value}";
    }

    public string CreateJsonKeyValuePair<T>(string key, T value) =>
        CreateJsonKeyValuePair(key, JsonValue.Create(value));

    public string CreateJsonKeyWithExplicitNullValue(string key)
    {
        key = MapStateOrScopeElementNames(key);
        return $"\"{key}\":null";
    }

    // builder

    public abstract FakeLoggerBuilder<TOptions> CreateLoggerBuilder();

    public abstract Action<TOptions> ConfigureIncludeEventHandling(bool includeEventHandling);
}
