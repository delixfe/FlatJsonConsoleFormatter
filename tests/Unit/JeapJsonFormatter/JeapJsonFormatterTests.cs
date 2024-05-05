using System.Collections.Concurrent;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using JsonConsoleFormatters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Unit.JeapJsonFormatter;

public class
    JeapJsonFormatterTests : FormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions,
    JeapJsonFormatterSpec>
{
    public JeapJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }

    [Theory]
    [CombinatorialData]
    public void Log_IgnoresTimestampFormatAndRespectsUseUtc(bool useUtcTimeStamp, bool unsafeRelaxedJson,
        [CombinatorialValues(null, "O", "hh:mm:ss")]
        string? timestampFormat)

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

        var expected = $"""
                        "{Spec.ElementNameTimestamp}":"{(useUtcTimeStamp ? C.ExpectedDefaultTimestampStringUtc : C.ExpectedDefaultTimestampStringLocal)}"
                        """;

        // Act
        logger.LogInformation("Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        logger.Formatted.Should().Contain(expected);
    }


    [Fact]
    public void Log_IncludeThreadName_OutputsThreadName()
    {
        var oldThreadName = Thread.CurrentThread.Name;

        try
        {
            // Arrange
            const string expectedName = "The sun is shining, the weather is sweet";
            Thread.CurrentThread.Name = expectedName;
            var logger = LoggerBuilder.With(o => o.IncludeThreadName = true).Build();

            // Act
            logger.LogInformation("Hi!");

            // Assert
            logger.Formatted.Should().BeValidJson();
            JToken.Parse(logger.Formatted!) //
                .Should().HaveElement(Spec.ElementNameThreadName).Which //
                .Should().BeOfType<JValue>().Which.Value.Should().Be(expectedName);
        }
        finally
        {
            Thread.CurrentThread.Name = oldThreadName;
        }
    }


    [Fact]
    public void Log_OutputsEventIdAndCategory()
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.LogInformation(new EventId(41, "FortyTwo"), "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();
        using var _ = new AssertionScope();
        var log = JToken.Parse(logger.Formatted!);
        logger.Formatted.Should().BeValidJson();

        log //
            .Should().HaveElement(Spec.ElementNameEventId).Which //
            .Should().BeOfType<JValue>().Which.Value.Should().Be(41);

        log //
            .Should().HaveElement(Spec.ElementNameEventName).Which //
            .Should().BeOfType<JValue>().Which.Value.Should().Be("FortyTwo");
    }

    [Theory]
    [CombinatorialData]
    public void Log_LevelIsOpenTelemetryCompatible(LogLevel level)
    {
        if (level == LogLevel.None) return;

        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.Log(level, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();

        var jToken = JToken.Parse(logger.Formatted!);
        var actualLevel = jToken[Spec.ElementNameLogLevel]?.Value<string>();

        OpenTelemetryAssertions.AssertShortNameIsMappedCorrectly(level, actualLevel!);
    }

    [Theory]
    [CombinatorialData]
    public void Log_SeverityIsOpenTelemetryCompatible(LogLevel level)
    {
        if (level == LogLevel.None) return;

        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.Log(level, "Hi!");

        // Assert
        logger.Formatted.Should().BeValidJson();

        var jToken = JToken.Parse(logger.Formatted!);
        jToken.Should().HaveElement(Spec.ElementNameSeverity);
        var actualSeverity = jToken[Spec.ElementNameSeverity]?.Value<int>();

        OpenTelemetryAssertions.AssertSeverityIsMappedCorrectly(level, actualSeverity!);
    }

    [Fact]
    public void MultiThreaded_ThreadLocalVariables()
    {
        const int threadCount = 10;
        const int iterationsPerThread = 1000;

        // Arrange
        ConcurrentQueue<string?> _formattedQueue = new();
        const int repeatKey = 10;
        var builder = LoggerBuilder //
            .With(ConfigActions.IncludeScopes) //
            .WithTestOutputHelper(null) // don't write all tests to the console
            .WithOnLogFormatted(msg => _formattedQueue.Enqueue(msg));

        Array.ForEach(Spec.AllElementNames, name => builder.AddStaticScope(null, name, "fromScope"));
        for (var i = 0; i < repeatKey; i++)
        {
            builder.AddStaticScope(null, "Key", "keyFromScope");
        }

        var logger = builder.Build();

        var msgBuilder = new StringBuilder();
        msgBuilder.Append("from message: ");
        Array.ForEach(Spec.AllElementNames, name => msgBuilder.Append($"{name}:{{{name}}} "));
        var msg = msgBuilder.ToString();
        var args = new object[Spec.AllElementNames.Length];
        Array.Fill(args, "fromMessage");
        var eventId = new EventId(42, "FortyTwo");
        var exception = new Exception("ex");
        var threads = new Thread[threadCount];

        // Act
        var action = () =>
        {
            for (var i = 0; i < iterationsPerThread; i++)
            {
                logger.LogInformation(eventId, exception, msg, args);
            }
        };

        for (var i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(new ThreadStart(action));
            threads[i].Start();
        }

        Array.ForEach(threads, t => t.Join());


        // Assert
        var formatteds = _formattedQueue.ToArray();
        formatteds.Should().HaveCount(threadCount * iterationsPerThread);

        var first = formatteds.First();
        TestOutputHelper.WriteLine(first);
        first.Should().BeValidJson();

        Parallel.For(1, threadCount * iterationsPerThread, (i, _) => Assert.Equal(first, formatteds[i]));
        foreach (var formatted in _formattedQueue)
        {
            // formatted.Should().Be(first);
            Assert.Equal(first, formatted);
        }
    }
}
