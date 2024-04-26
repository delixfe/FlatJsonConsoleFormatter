using System.Text.Encodings.Web;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Json;
using JsonConsoleFormatters;
using Microsoft.Extensions.Logging;
using Unit.Infrastructure;
using Xunit.Abstractions;

namespace Unit;

public class JeapJsonFormatterTests : FormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(FakeLoggerBuilder.JeapJson(),
        testOutputHelper)
    {
    }

    [Theory]
    [CombinatorialData]
    public void Log_IgnoresTimestampFormatAndRespectsUseUtc(bool useUtcTimeStamp, bool unsafeRelaxedJson,
        [CombinatorialValues(null, "O", "hh:mm:ss")] string? timestampFormat)

    {
        // Arrange
        var logger = LoggerBuilder
            .With(o =>
            {
                o.TimestampFormat = timestampFormat;
                o.UseUtcTimestamp = useUtcTimeStamp;
                o.JsonWriterOptions = new JsonWriterOptions
                {
                    // otherwise escapes for timezone formatting from + to \u002b
                    Encoder = unsafeRelaxedJson
                        ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                        : JavaScriptEncoder.Default, //  
                    Indented = false
                };
            })
            .Build();
        var expectedElementName = "Timestamp";
        var expectedTimestamp =
            useUtcTimeStamp ? C.ExpectedDefaultTimestampStringUtc : C.ExpectedDefaultTimestampStringLocal;
        var expected = $"""
                        "{expectedElementName}":"{expectedTimestamp}"
                        """;

        TestOutputHelper.WriteLine($"UTC:   {C.DefaultStartTimestamp.ToUniversalTime():O}");
        TestOutputHelper.WriteLine($"Local: {C.DefaultStartTimestamp.ToLocalTime():O}");
        TestOutputHelper.WriteLine($"Expected: {expected}");


        // Act
        logger.LogInformation("Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        logger.Formatted.Should().Contain(expected);
    }
}
