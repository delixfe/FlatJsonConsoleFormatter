using Microsoft.Extensions.Logging;

namespace Unit;

public static class Data
{
    public static TheoryData<LogLevel> LogLevels
    {
        get
        {
            var data = new TheoryData<LogLevel>();
            foreach (LogLevel level in Enum.GetValues(typeof(LogLevel)))
            {
                if (level == LogLevel.None)
                {
                    continue;
                }

                data.Add(level);
            }

            return data;
        }
    }

    public static TheoryData<object?, string> SpecialCaseValues
    {
        get
        {
            var data = new TheoryData<object?, string>
            {
                // primitives, excluding floating point
                { true, "true" },
                { (byte)1, "1" },
                { (sbyte)1, "1" },
                { 'a', "\"a\"" },
                { 1, "1" },
                { (uint)1, "1" },
                { (long)1, "1" },
                { (ulong)1, "1" },
                { (short)1, "1" },
                { (ushort)1, "1" },
                { 1.2m, "1.2" },

                // nullables primitives, excluding floating point
                { (bool?)true, "true" },
                { (byte?)1, "1" },
                { (sbyte?)1, "1" },
                { (char?)'a', "\"a\"" },
                { (int?)1, "1" },
                { (uint?)1, "1" },
                { (long?)1, "1" },
                { (ulong?)1, "1" },
                { (short?)1, "1" },
                { (ushort?)1, "1" },
                { (decimal?)1.2m, "1.2" },

                // Dynamic object serialized as string
                { new { a = 1, b = 2 }, "\"{ a = 1, b = 2 }\"" },

                // null should not be serialized as special string in the state value, only in message
                { null, "null" }
            };
            return data;
        }
    }

    public static TheoryData<object> FloatingPointValues
    {
        get
        {
            var data = new TheoryData<object>
            {
                1.2,
                1.2f,

                // nullables
                (double?)1.2,
                (float?)1.2f
            };
            return data;
        }
    }
}
