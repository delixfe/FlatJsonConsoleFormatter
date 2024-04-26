using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace JsonConsoleFormatters;

public sealed class JeapJsonConsoleFormatter : ConsoleFormatter, IDisposable
{
    public const string FormatterName = "jeap-json";

    private static readonly JsonNamingPolicy JsonCamelCaseNamingPolicy = JsonNamingPolicy.CamelCase;

    private readonly IDisposable? _optionsReloadToken;

    public JeapJsonConsoleFormatter(IOptionsMonitor<JeapJsonConsoleFormatterOptions> options, TimeProvider timeProvider)
        : base(FormatterName)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        TimeProvider = timeProvider;
    }

    internal JeapJsonConsoleFormatterOptions FormatterOptions { get; set; }
    public TimeProvider TimeProvider { get; }

    public void Dispose() => _optionsReloadToken?.Dispose();

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        const int DefaultBufferSize = 1024;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize))
        {
            using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
            {
                var messageProperties = new Dictionary<string, object?>();

                writer.WriteStartObject();

                // we ignore the TimestampFormat option, one could check for TimestampFormat == "O"
                writer.WriteString("@timestamp"u8,
                    FormatterOptions.UseUtcTimestamp ? TimeProvider.GetUtcNow() : TimeProvider.GetLocalNow());

                writer.WriteString("level"u8, GetLogLevelString(logEntry.LogLevel));

                writer.WriteString("logger"u8, logEntry.Category);

                writer.WriteString("message"u8, message);


                if (FormatterOptions.IncludeEventId)
                    writer.WriteNumber("eventId"u8, logEntry.EventId.Id);

                if (FormatterOptions.IncludeEventId && logEntry.EventId.Name is not null)
                    writer.WriteString("eventName"u8, logEntry.EventId.Name);

                if (logEntry.Exception != null)
                    writer.WriteString("exception"u8, logEntry.Exception.ToString());

                AddScopeInformation(messageProperties, writer, scopeProvider);
                if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object>> stateProperties)
                {
                    foreach (var item in stateProperties)
                    {
                        if (item.Key != "{OriginalFormat}")
                            AddMessageProperty(messageProperties, item.Key, item.Value);
                    }
                }

                foreach (var prop in messageProperties)
                    WriteItem(writer, prop);

                writer.WriteEndObject();
                writer.Flush();
            }

            textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span));
        }

        textWriter.Write(Environment.NewLine);
    }

    private void AddMessageProperty(Dictionary<string, object?> messageProperties, string key, object? value)
    {
        if (FormatterOptions.MergeDuplicateKeys)
        {
            messageProperties[key] = value;
        }
        else
        {
            var k = key;
            var n = 1;
            while (messageProperties.ContainsKey(k))
                k = $"{key}_{n++}";
            messageProperties.Add(k, value);
        }
    }

    private static ReadOnlySpan<byte> GetLogLevelString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "TRACE"u8,
            LogLevel.Debug => "DEBUG"u8,
            LogLevel.Information => "INFO"u8,
            LogLevel.Warning => "WARN"u8,
            LogLevel.Error => "ERROR"u8,
            LogLevel.Critical => "CRITIC"u8,
            _ => ThrowLogLevelArgumentOutOfRangeException(nameof(logLevel), logLevel)
        };


    [DoesNotReturn]
    private static ReadOnlySpan<byte> ThrowLogLevelArgumentOutOfRangeException(string paraName, LogLevel logLevel) =>
        throw new ArgumentOutOfRangeException(paraName, logLevel,
            $"{nameof(LogLevel)} does not contain a value for {logLevel}");

    private void AddScopeInformation(Dictionary<string, object?> messageProperties, Utf8JsonWriter writer,
        IExternalScopeProvider? scopeProvider)
    {
        var scopeNum = 0;
        if (FormatterOptions.IncludeScopes && scopeProvider != null)
        {
            scopeProvider.ForEachScope((scope, _) =>
            {
                if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
                {
                    foreach (var item in scopeItems)
                    {
                        AddMessageProperty(messageProperties, item.Key, item.Value);
                    }
                }
                else
                {
                    AddMessageProperty(messageProperties, $"Scope{scopeNum++}", ToInvariantString(scope));
                }
            }, writer);
        }
    }

    private static void WriteItem(Utf8JsonWriter writer, KeyValuePair<string, object?> item)
    {
        var key = item.Key;
        switch (item.Value)
        {
            case bool boolValue:
                writer.WriteBoolean(key, boolValue);
                break;
            case byte byteValue:
                writer.WriteNumber(key, byteValue);
                break;
            case sbyte sbyteValue:
                writer.WriteNumber(key, sbyteValue);
                break;
            case char charValue:
#if NETCOREAPP
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
#else
                    writer.WriteString(key, charValue.ToString());
#endif
                break;
            case decimal decimalValue:
                writer.WriteNumber(key, decimalValue);
                break;
            case double doubleValue:
                writer.WriteNumber(key, doubleValue);
                break;
            case float floatValue:
                writer.WriteNumber(key, floatValue);
                break;
            case int intValue:
                writer.WriteNumber(key, intValue);
                break;
            case uint uintValue:
                writer.WriteNumber(key, uintValue);
                break;
            case long longValue:
                writer.WriteNumber(key, longValue);
                break;
            case ulong ulongValue:
                writer.WriteNumber(key, ulongValue);
                break;
            case short shortValue:
                writer.WriteNumber(key, shortValue);
                break;
            case ushort ushortValue:
                writer.WriteNumber(key, ushortValue);
                break;
            case null:
                writer.WriteNull(key);
                break;
            default:
                writer.WriteString(key, ToInvariantString(item.Value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(JeapJsonConsoleFormatterOptions options) => FormatterOptions = options;
}
