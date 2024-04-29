using System.Runtime.CompilerServices;
using Benchmarks.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Benchmarks.Scenarios;

public static partial class TypeBased
{
    public const int DefinitionCount = 8;

    private static readonly DateTimeOffset _dateTimeOffset =
        new(2021, 8, 30, 13, 41, 33, 876, 543, TimeSpan.FromHours(2));

    private static readonly DateOnly _dateOnly = new(2021, 8, 30);

    private static readonly TimeSpan _timeSpan =
        new DateTimeOffset(2021, 8, 1, 0, 0, 0, TimeSpan.Zero).Subtract(_dateTimeOffset);

    private static readonly DateTime _dateTime = new(2021, 8, 30, 13, 41, 33, 876, DateTimeKind.Local);
    private static readonly TimeOnly _timeOnly = new(13, 41, 33, 876);
    private static readonly Guid _guid = new("caca0399-274a-4fe4-81af-9ddd42b4a22d");
    private static readonly Guid[] _guidArray = { _guid, _guid, _guid, _guid, _guid };

    private static readonly Uri _uri =
        new UriBuilder("https", "some.host.some.where.example.test", 80, "/not/so/short/a/path/it/is").Uri;

    private static readonly Uri[] _uriArray = { _uri, _uri, _uri, _uri, _uri };
    private static readonly char[] _charArray = Lorem.Ipsum.ToArray();
    private static readonly char[] _charArrayShort = "Hello World!".ToCharArray();

    [LoggerMessage(
        "primitives: {String}, {Byte}, {SByte}, {Char}, {Int}, {UInt}, {Long}, {ULong}, {Short}, {UShort}, {Double}, {Float}, {Decimal}, {Bool}")] //", {Bool}, {Guid}, {DateTime}, {DateTimeOffset}, {TimeSpan}, {Uri}, {Enum}, {Object}, {Exception}")]
    static partial void LogPrimitives(ILogger logger, LogLevel logLevel, string @string, byte @byte, sbyte @sbyte,
        char @char, int @int, uint @uint, long @long, ulong @ulong, short @short, ushort @ushort, double @double,
        float @float, decimal @decimal, bool @bool);

    [LoggerMessage(
        "nullable primitives: {StringN}, {ByteN}, {SByteN}, {CharN}, {IntN}, {UIntN}, {LongN}, {ULongN}, {ShortN}, {UShortN}, {DoubleN}, {FloatN}, {DecimalN}, {BoolN}")]
    static partial void LogNullablePrimitives(ILogger logger, LogLevel logLevel, string? stringN, byte? byteN,
        sbyte? sbyteN, char? charN, int? intN, uint? uintN, long? longN, ulong? ulongN, short? shortN, ushort? ushortN,
        double? doubleN, float? floatN, decimal? decimalN, bool? boolN);

    [LoggerMessage("datetime variants: {DateTime}, {DateTimeOffset}, {TimeSpan}, {DateOnly}, {TimeOnly}")]
    static partial void LogDateTimeVariants(ILogger logger, LogLevel logLevel, DateTime dateTime,
        DateTimeOffset dateTimeOffset, TimeSpan timeSpan, DateOnly dateOnly, TimeOnly timeOnly);

    [LoggerMessage("nullable datetime variants: {DateTimeN}, {DateTimeOffsetN}, {TimeSpanN}, {DateOnlyN}, {TimeOnlyN}")]
    static partial void LogNullableDateTimeVariants(ILogger logger, LogLevel logLevel, DateTime? dateTimeN,
        DateTimeOffset? dateTimeOffsetN, TimeSpan? timeSpanN, DateOnly? dateOnlyN, TimeOnly? timeOnlyN);

    [LoggerMessage("structs: {Guid}")]
    static partial void LogStructs(ILogger logger, LogLevel logLevel, Guid guid);

    [LoggerMessage("nullable structs: {GuidN}")]
    static partial void LogNullableStructs(ILogger logger, LogLevel logLevel, Guid? guidN);

    [LoggerMessage("reference types: {Uri}, {ArrayChar}, {ArrayGuid}, {ArrayUri}")]
    static partial void LogReferenceTypes(ILogger logger, LogLevel logLevel, Uri uri, char[] arrayChar,
        Guid[] arrayGuid, Uri[] arrayUri);

    // TODO: add enums: custom serializer, flags

    [LoggerMessage("Dynamic object serialized as string: {Dynamic}")]
    static partial void LogDynamic(ILogger logger, LogLevel logLevel, dynamic dynamic);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEachDefinition(ILogger logger)
    {
        LogPrimitives(logger, LogLevel.Trace, "string", 1, 1, 'a', int.MinValue, uint.MaxValue, long.MinValue,
            ulong.MaxValue, short.MinValue, ushort.MaxValue, 1.237773873263, 1.232323f, 1.2343418236487316246m, true);
        LogNullablePrimitives(logger, LogLevel.Trace, "string", 1, 1, 'a', int.MinValue, uint.MaxValue, long.MinValue,
            ulong.MaxValue, short.MinValue, ushort.MaxValue, 1.237773873263, 1.232323f, 1.2343418236487316246m, true);
        LogDateTimeVariants(logger, LogLevel.Warning, _dateTime, _dateTimeOffset, _timeSpan, _dateOnly, _timeOnly);
        LogNullableDateTimeVariants(logger, LogLevel.Warning, _dateTime, _dateTimeOffset, _timeSpan, _dateOnly,
            _timeOnly);
        LogStructs(logger, LogLevel.Critical, _guid);
        LogNullableStructs(logger, LogLevel.Error, _guid);
        LogReferenceTypes(logger, LogLevel.Warning, _uri, _charArray, _guidArray, _uriArray);
        LogDynamic(logger, LogLevel.Information,
            new { a = 1, b = 2, c = new { d = 3, dto = _dateTimeOffset, chars = _charArrayShort } });
    }
}
