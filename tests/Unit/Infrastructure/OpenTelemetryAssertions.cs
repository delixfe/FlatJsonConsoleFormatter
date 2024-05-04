using Microsoft.Extensions.Logging;

namespace Unit.Infrastructure;

public static class OpenTelemetryAssertions
{
    public static void AssertSeverityIsMappedCorrectly(LogLevel logLevel, int? actualSeverity)
    {
        var expectedSeverity = MapToLogRecordSeverity(logLevel);
        actualSeverity.Should().NotBeNull();
        actualSeverity.Should().Be((int)expectedSeverity);
    }

    public static void AssertShortNameIsMappedCorrectly(LogLevel logLevel, string? actualShortName)
    {
        var expectedSeverity = MapToLogRecordSeverity(logLevel);
        var expectedShortName = LogRecordSeverityExtensions.ToShortName(expectedSeverity);

        actualShortName.Should().NotBeNull();
        actualShortName.Should().Be(expectedShortName);
    }

    #region OpenTelemetryImplementation

    // taken from https://github.com/open-telemetry/opentelemetry-dotnet/blob/13791e268e9ca6744f64c74951b718a8c7002bcc/src/OpenTelemetry.Api/Logs/LogRecordSeverity.cs

    internal enum LogRecordSeverity
    {
        /// <summary>Unspecified severity (0).</summary>
        Unspecified = 0,

        /// <summary>Trace severity (1).</summary>
        Trace = 1,

        /// <summary>Trace1 severity (2).</summary>
        Trace2 = Trace + 1,

        /// <summary>Trace3 severity (3).</summary>
        Trace3 = Trace2 + 1,

        /// <summary>Trace4 severity (4).</summary>
        Trace4 = Trace3 + 1,

        /// <summary>Debug severity (5).</summary>
        Debug = 5,

        /// <summary>Debug2 severity (6).</summary>
        Debug2 = Debug + 1,

        /// <summary>Debug3 severity (7).</summary>
        Debug3 = Debug2 + 1,

        /// <summary>Debug4 severity (8).</summary>
        Debug4 = Debug3 + 1,

        /// <summary>Info severity (9).</summary>
        Info = 9,

        /// <summary>Info2 severity (11).</summary>
        Info2 = Info + 1,

        /// <summary>Info3 severity (12).</summary>
        Info3 = Info2 + 1,

        /// <summary>Info4 severity (13).</summary>
        Info4 = Info3 + 1,

        /// <summary>Warn severity (13).</summary>
        Warn = 13,

        /// <summary>Warn2 severity (14).</summary>
        Warn2 = Warn + 1,

        /// <summary>Warn3 severity (15).</summary>
        Warn3 = Warn2 + 1,

        /// <summary>Warn severity (16).</summary>
        Warn4 = Warn3 + 1,

        /// <summary>Error severity (17).</summary>
        Error = 17,

        /// <summary>Error2 severity (18).</summary>
        Error2 = Error + 1,

        /// <summary>Error3 severity (19).</summary>
        Error3 = Error2 + 1,

        /// <summary>Error4 severity (20).</summary>
        Error4 = Error3 + 1,

        /// <summary>Fatal severity (21).</summary>
        Fatal = 21,

        /// <summary>Fatal2 severity (22).</summary>
        Fatal2 = Fatal + 1,

        /// <summary>Fatal3 severity (23).</summary>
        Fatal3 = Fatal2 + 1,

        /// <summary>Fatal4 severity (24).</summary>
        Fatal4 = Fatal3 + 1
    }

    // taken from https://github.com/open-telemetry/opentelemetry-dotnet/blob/13791e268e9ca6744f64c74951b718a8c7002bcc/src/OpenTelemetry.Api/Logs/LogRecordSeverityExtensions.cs
    internal static class LogRecordSeverityExtensions
    {
        internal const string UnspecifiedShortName = "UNSPECIFIED";

        internal const string TraceShortName = "TRACE";
        internal const string Trace2ShortName = TraceShortName + "2";
        internal const string Trace3ShortName = TraceShortName + "3";
        internal const string Trace4ShortName = TraceShortName + "4";

        internal const string DebugShortName = "DEBUG";
        internal const string Debug2ShortName = DebugShortName + "2";
        internal const string Debug3ShortName = DebugShortName + "3";
        internal const string Debug4ShortName = DebugShortName + "4";

        internal const string InfoShortName = "INFO";
        internal const string Info2ShortName = InfoShortName + "2";
        internal const string Info3ShortName = InfoShortName + "3";
        internal const string Info4ShortName = InfoShortName + "4";

        internal const string WarnShortName = "WARN";
        internal const string Warn2ShortName = WarnShortName + "2";
        internal const string Warn3ShortName = WarnShortName + "3";
        internal const string Warn4ShortName = WarnShortName + "4";

        internal const string ErrorShortName = "ERROR";
        internal const string Error2ShortName = ErrorShortName + "2";
        internal const string Error3ShortName = ErrorShortName + "3";
        internal const string Error4ShortName = ErrorShortName + "4";

        internal const string FatalShortName = "FATAL";
        internal const string Fatal2ShortName = FatalShortName + "2";
        internal const string Fatal3ShortName = FatalShortName + "3";
        internal const string Fatal4ShortName = FatalShortName + "4";

        private static readonly string[] LogRecordSeverityShortNames =
        {
            UnspecifiedShortName, TraceShortName, Trace2ShortName, Trace3ShortName, Trace4ShortName, DebugShortName,
            Debug2ShortName, Debug3ShortName, Debug4ShortName, InfoShortName, Info2ShortName, Info3ShortName,
            Info4ShortName, WarnShortName, Warn2ShortName, Warn3ShortName, Warn4ShortName, ErrorShortName,
            Error2ShortName, Error3ShortName, Error4ShortName, FatalShortName, Fatal2ShortName, Fatal3ShortName,
            Fatal4ShortName
        };

        internal static string ToShortName(LogRecordSeverity logRecordSeverity)
        {
            var severityLevel = (int)logRecordSeverity;

            if (severityLevel < 0 || severityLevel > 24)
            {
                severityLevel = 0;
            }

            return LogRecordSeverityShortNames[severityLevel];
        }
    }

    // adapted from https://github.com/open-telemetry/opentelemetry-dotnet/blob/5bcc8052808326c9ff012233eaf56e072afa61ad/src/OpenTelemetry/Logs/ILogger/OpenTelemetryLogger.cs#L119 
    internal static LogRecordSeverity MapToLogRecordSeverity(LogLevel logLevel)
    {
        var intLogLevel = (uint)logLevel;
        if (intLogLevel < 6)
        {
            return (LogRecordSeverity)((intLogLevel * 4) + 1);
        }

        throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "Invalid log level.");
    }

    #endregion
}
