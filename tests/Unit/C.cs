namespace Unit;

/// <summary>
///     Well-known constants
/// </summary>
public static class C
{
    public const string DefaultLoggerName = "test";
    public const string ExpectedDefaultTimestampStringLocal = "2021-08-30T15:41:33.876543+02:00";
    public const string ExpectedDefaultTimestampStringUtc = "2021-08-30T13:41:33.876543+00:00";
    public static readonly DateTimeOffset DefaultStartTimestamp = new(2021, 8, 30, 13, 41, 33, 876, 543, TimeSpan.Zero);
    public static readonly TimeZoneInfo DefaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Zurich");
}
