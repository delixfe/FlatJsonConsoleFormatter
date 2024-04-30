﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace JsonConsoleFormatters;

/// <summary>
///     An opinionated log message formatter based on SprintBoot's default log message format
/// </summary>
[ProviderAlias(FormatterName)]
public sealed class JeapJsonConsoleFormatter : ConsoleFormatter, IDisposable
{
    /// <summary>
    ///     The name of the formatter.
    /// </summary>
    public const string FormatterName = "jeap-json";

    private static readonly JsonNamingPolicy JsonCamelCaseNamingPolicy = JsonNamingPolicy.CamelCase;

    [ThreadStatic] private static HashSet<string>? writtenNames;

    private readonly IDisposable? _optionsReloadToken;
    private readonly TimeProvider _timeProvider;

    internal JeapJsonConsoleFormatter(IOptionsMonitor<JeapJsonConsoleFormatterOptions> options,
        TimeProvider timeProvider)
        : base(FormatterName)
    {
        ReloadLoggerOptions(options.CurrentValue);
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _timeProvider = timeProvider;
    }

    internal JeapJsonConsoleFormatterOptions FormatterOptions { get; set; }

    /// <summary>
    ///     Disposes the formatter and releases any resources it is using.
    /// </summary>
    public void Dispose() => _optionsReloadToken?.Dispose();

    /// <summary>
    ///     Writes a formatted log entry to the <paramref name="textWriter" />.
    /// </summary>
    /// <param name="logEntry">The log entry to write.</param>
    /// <param name="scopeProvider">The provider for the current scope.</param>
    /// <param name="textWriter">The writer to use to write the log entry.</param>
    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }

        writtenNames?.Clear();
        // TODO(perf): we cannot detect instances with huge capacities, so how can we reset the capacity to 32
        writtenNames ??= new HashSet<string>(32);


        const int DefaultBufferSize = 1024;
        using (var output = new PooledByteBufferWriter(DefaultBufferSize))
        {
            using (var writer = new Utf8JsonWriter(output, FormatterOptions.JsonWriterOptions))
            {
                writer.WriteStartObject();

                // we ignore the TimestampFormat option, one could check for TimestampFormat == "O"
                writer.WriteString("@timestamp"u8,
                    FormatterOptions.UseUtcTimestamp ? _timeProvider.GetUtcNow() : _timeProvider.GetLocalNow());

                writer.WriteString("level"u8, GetLogLevelString(logEntry.LogLevel));

                writer.WriteString("logger"u8, logEntry.Category);

                if (FormatterOptions.IncludeThreadName)
                    writer.WriteString("thread_name"u8, Thread.CurrentThread.Name);

                writer.WriteString("message"u8, message);


                if (logEntry.EventId.Id > 0)
                    writer.WriteNumber("eventId"u8, logEntry.EventId.Id);

                if (!string.IsNullOrEmpty(logEntry.EventId.Name))
                    writer.WriteString("eventName"u8, logEntry.EventId.Name);

                if (logEntry.Exception != null)
                    writer.WriteString("exception"u8, logEntry.Exception.ToString());


                // we handle scopes first, so that these attribute names remain stable(ish)
                AddScopeInformation(writer, scopeProvider, writtenNames);

                if (logEntry.State is IReadOnlyCollection<KeyValuePair<string, object?>> stateProperties)
                {
                    foreach (var (key, value) in stateProperties)
                    {
                        if (key != "{OriginalFormat}")
                            WriteItem(writer, GetUniqueKey(key, writtenNames), value);
                    }
                }

                writer.WriteEndObject();
                writer.Flush();
            }

            textWriter.Write(Encoding.UTF8.GetString(output.WrittenMemory.Span));
        }

        textWriter.Write(Environment.NewLine);
    }


    private static bool IsReservedKey(string key) => key switch
    {
        // we need to use the original names, as we test with unmapped namecs
        "Timestamp" => true,
        "LogLevel" => true,
        "Category" => true,
        "Message" => true,
        "EventId" => true,
        "EventName" => true,
        "Exception" => true,
        _ => false
    };

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


    private void AddScopeInformation(Utf8JsonWriter writer, IExternalScopeProvider? scopeProvider,
        HashSet<string> writtenNames)
    {
        if (!FormatterOptions.IncludeScopes || scopeProvider == null)
            return;
        var scopeNum = 0;
        scopeProvider.ForEachScope((scope, _) =>
        {
            if (scope is IEnumerable<KeyValuePair<string, object>> scopeItems)
            {
                foreach (var item in scopeItems)
                {
                    WriteItem(writer, GetUniqueKey(item.Key, writtenNames), item.Value);
                }
            }
            else
            {
                // TODO: always output if scope messages are enabled
                WriteItem(writer, GetUniqueKey($"scope_{scopeNum}", writtenNames), ToInvariantString(scope));
            }

            scopeNum += 1;
        }, writer);
    }

    private static string GetUniqueKey(string key, HashSet<string> writtenNames)
    {
        var result = key;

        if (IsReservedKey(key) || writtenNames.Contains(key))
        {
            var index = 0;
            do
            {
                index += 1;
                result = $"{key}_{index}";
            } while (writtenNames.Contains(result) || IsReservedKey(result));
        }

        writtenNames.Add(result);
        return result;
    }

    private static void WriteItem(Utf8JsonWriter writer, string key, object? value)
    {
        // TODO(perf): cache mapped keys
        key = JsonCamelCaseNamingPolicy.ConvertName(key);
        switch (value)
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
                writer.WriteString(key, MemoryMarshal.CreateSpan(ref charValue, 1));
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
                writer.WriteString(key, ToInvariantString(value));
                break;
        }
    }

    private static string? ToInvariantString(object? obj) => Convert.ToString(obj, CultureInfo.InvariantCulture);

    [MemberNotNull(nameof(FormatterOptions))]
    private void ReloadLoggerOptions(JeapJsonConsoleFormatterOptions options) => FormatterOptions = options;
}
