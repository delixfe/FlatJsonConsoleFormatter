using System.Text.Encodings.Web;
using FlatJsonConsoleFormatter;
using Microsoft.Extensions.Logging.Console;

namespace Unit.Infrastructure;

public static class ConfigActions
{
    public static readonly Action<ConsoleFormatterOptions> TimestampFormatO = o => o.TimestampFormat = "O";
    public static readonly Action<ConsoleFormatterOptions> UseUtcTimestamp = o => o.UseUtcTimestamp = false;
    public static readonly Action<ConsoleFormatterOptions> IncludeScopes = o => o.IncludeScopes = true;
    public static readonly Action<ConsoleFormatterOptions> DontIncludeScopes = o => o.IncludeScopes = false;

    public static readonly Action<JsonConsoleFormatterOptions> UnsafeRelaxedJsonEscaping = o =>
        o.JsonWriterOptions = o.JsonWriterOptions with { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public static readonly Action<JsonConsoleFormatterOptions> Indented = o =>
        o.JsonWriterOptions = o.JsonWriterOptions with { Indented = true };

    public static readonly Action<FlatJsonConsoleFormatterOptions> TruncateCategory = o => o.TruncateCategory = true;
    public static readonly Action<FlatJsonConsoleFormatterOptions> IncludeEventId = o => o.IncludeEventId = true;

    public static readonly Action<FlatJsonConsoleFormatterOptions>
        MergeDuplKeys = o => o.MergeDuplicateKeys = true;
}
