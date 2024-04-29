using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class
    ExtensionsLoggingJsonConsoleFormatterTestsBase<TFormatter, TFormatterOptions> : FormatterTestsBase<TFormatter,
    TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected const string _loggerName = C.DefaultLoggerName;
    // protected const string _state = "This is a test, and {curly braces} are just fine!"; 

    protected ExtensionsLoggingJsonConsoleFormatterTestsBase(SpecBase<TFormatterOptions> spec,
        ITestOutputHelper testOutputHelper) : base(spec, testOutputHelper)
    {
    }

    // TODO: Log_ExceptionWithMessage_ExtractsInfo

    // TODO: Log_IncludeScopes_ContainsDuplicateNamedPropertiesInScope_AcceptableJson


    // TODO: try to understand, what is the purpose of this test
    [Fact(Skip = "Test name is a misnomer, as there is scope")]
    public void NoLogScope_DoesNotWriteAnyScopeContentToOutput_Json()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o =>
            {
                o.IncludeScopes = true;
                o.JsonWriterOptions = new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            })
            .Build();

        // Act
        using (logger.BeginScope("Scope with named parameter {namedParameter}", 123))
        using (logger.BeginScope("SimpleScope"))
            logger.Log(LogLevel.Warning, 0, "Message with {args}", 73);

        // Assert
        Assert.Contains("Message with {args}", logger.Formatted);
        Assert.Contains("73", logger.Formatted);
        Assert.Contains("{OriginalFormat}", logger.Formatted);
        Assert.Contains("namedParameter", logger.Formatted);
        Assert.Contains("123", logger.Formatted);
        Assert.Contains("SimpleScope", logger.Formatted);
    }


    [Fact]
    public virtual void Log_TimestampFormatSet_ContainsTimestamp()
    {
        // Arrange
        var logger = LoggerBuilder
            .With(o => o.TimestampFormat = "hh:mm:ss ")
            .Build();

        // Act
        logger.LogCritical(0, null);

        // Assert
        logger.Formatted.Should().BeValidJson() //
            .Subject.Should().HaveElement(Spec.ElementNameTimestamp).Which.Should().MatchRegex(@"\d{2}:\d{2}:\d{2}");
    }

    [Theory]
    [CombinatorialData]
    public void Log_NullMessage_LogsWhenMessageIsNotProvided(bool withException)
    {
        // Arrange
        var logger = LoggerBuilder.Build();
        var exception = withException ? new InvalidOperationException("Invalid value") : null;

        // Act
        logger.LogCritical(exception, null);

        // Assert
        logger.Formatted.Should().BeValidJson();
    }

    [Fact]
    public virtual void Log_StateAndScopeAreCollections_IncludesMessageAndCollectionValues()
    {
        // Arrange
        var logger = LoggerBuilder.With(ConfigActions.IncludeScopes).Build();

        // Act
        using (logger.BeginScope("{Number}", 2))
        using (logger.BeginScope("{AnotherNumber}", 3))
        {
            logger.LogInformation("{LogEntryNumber}", 1);
        }

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;

        message.Should().Contain($"\"{Spec.ElementNameMessage}\":\"1\"");
        if (Spec.OutputsOriginalFormat)
            message.Should().Contain($"\"{Spec.ElementNameOriginalFormat}\":\"{{LogEntryNumber}}\"");

        message.Should().Contain(JsonKeyValue("Number", 2));
        message.Should().Contain(JsonKeyValue("AnotherNumber", 3));

        // depending on the handling of duplicate element names, we cannot validate scope's message or template
        if (Spec.ScopeOutputsOriginalFormat || Spec.ScopeOutputsMessage)
            Assert.Fail(
                "Does not know how to validate scope's message or OriginalFormat due to the handling of duplicate element names");

        // Assert.Equal(
        //     "{\"EventId\":0,\"LogLevel\":\"Information\",\"Category\":\"test\""
        //     + ",\"Message\":\"1\""
        //     + ",\"State\":{\"Message\":\"1\",\"LogEntryNumber\":1,\"{OriginalFormat}\":\"{LogEntryNumber}\"}"
        //     + ",\"Scopes\":[{\"Message\":\"2\",\"Number\":2,\"{OriginalFormat}\":\"{Number}\"},{\"Message\":\"3\",\"AnotherNumber\":3,\"{OriginalFormat}\":\"{AnotherNumber}\"}]"
        //     + "}" + Environment.NewLine,
        //     GetMessage(sink.Writes.GetRange(0 * t.WritesPerMsg, t.WritesPerMsg)));
    }


    [Theory]
    [MemberData(nameof(Data.SpecialCaseValues), MemberType = typeof(Data))]
    public void Log_StateAndScopeContainsSpecialCaseValue_SerializesValueAsExpected(object value,
        string expectedJsonValue)
    {
        // Arrange
        var logger = LoggerBuilder.With(ConfigActions.IncludeScopes).Build();

        // Act
        using (logger.BeginScope("{Value}", value))
        {
            logger.LogInformation("{LogEntryValue}", value);
        }

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;

        // Adaption: don't check for the trailing comma
        message.Should().Contain(JsonKeyValue("Value", expectedJsonValue));
        message.Should().Contain(JsonKeyValue("LogEntryValue", expectedJsonValue));

        // Assert.Contains("\"Value\":" + expectedJsonValue + ",", logger.Formatted);
        // Assert.Contains("\"LogEntryValue\":" + expectedJsonValue + ",", logger.Formatted);
    }

    [Theory]
    [MemberData(nameof(Data.FloatingPointValues), MemberType = typeof(Data))]
    public void Log_StateAndScopeContainsFloatingPointType_SerializesValue(object value)
    {
        // Arrange
        // Arrange
        var logger = LoggerBuilder.With(ConfigActions.IncludeScopes).Build();


        // Act
        using (logger.BeginScope("{Value}", value))
        {
            logger.LogInformation("{LogEntryValue}", value);
        }

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        AssertMessageValue(message, Spec.MapStateOrScopeElementNames("Value"));
        AssertMessageValue(message, Spec.MapStateOrScopeElementNames("LogEntryValue"));

        static void AssertMessageValue(string message, string propertyName)
        {
            // Adaption: accept comma or }
            // var serializedValueMatch = Regex.Match(message, "\"" + propertyName + "\":(.*?),");
            var serializedValueMatch = Regex.Match(message, "\"" + propertyName + "\":(.*?)[,}]");
            Assert.Equal(2, serializedValueMatch.Groups.Count);
            var jsonValue = serializedValueMatch.Groups[1].Value;
            Assert.True(
                double.TryParse(jsonValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var floatingPointValue),
                "The json does not contain a floating point value: " + jsonValue);
            Assert.Equal(1.2, floatingPointValue, 2);
        }
    }

    [Fact]
    public void Log_StateAndScopeContainsNullValue_SerializesNull()
    {
        // Arrange
        var logger = LoggerBuilder.With(ConfigActions.IncludeScopes).Build();

        // Act
        using (logger.BeginScope(new WithNullValue("ScopeKey")))
        {
            logger.Log(LogLevel.Information, 0, new WithNullValue("LogKey"), null,
                (a, b) => string.Empty);
        }

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        var x = JsonKeyValue("ScopeKey", (string?)null);
        Assert.Contains(Spec.CreateJsonKeyWithExplicitNullValue("ScopeKey"), message);
        Assert.Contains(Spec.CreateJsonKeyWithExplicitNullValue("LogKey"), message);
    }

    [Fact]
    public void Log_ScopeIsIEnumerable_SerializesKeyValuePair()
    {
        // Arrange
        var logger = LoggerBuilder.With(ConfigActions.IncludeScopes).Build();

        // Act
        using (logger.BeginScope(new[] { 2 }.Select(x => new KeyValuePair<string, object>("Value", x))))
        {
            logger.LogInformation("{LogEntryNumber}", 1);
        }

        // Assert
        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        if (Spec.ScopeOutputsMessage)
            Assert.Contains(JsonKeyValue("Value", 2), message);
    }

    [Theory]
    [CombinatorialData]
    public void ShouldContainInnerException(bool indented)
    {
        var logger = LoggerBuilder.With(indented ? ConfigActions.Indented : ConfigActions.NoOp).Build();
        var innerException = new ArgumentException("inner").EnsureStackTrace();
        var rootException = new InvalidOperationException("root", innerException).EnsureStackTrace();

        // Act
        logger.LogError(rootException, null);

        // Assert
        // Adaptions: only checks if exceptions are present via type and message

        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;
        using var _ = new AssertionScope();

        message.Should().Contain(rootException.GetType().FullName).And.Contain(rootException.Message);
        message.Should().Contain(innerException.GetType().FullName).And.Contain(innerException.Message);

        // Assert.Contains(rootException.Message, json);
        // Assert.Contains(rootException.InnerException.Message, json);
        //
        // Assert.Contains(GetContent(rootException), json);
        // Assert.Contains(GetContent(rootException.InnerException), json);
    }


    [Theory]
    [CombinatorialData]
    public void ShouldContainAggregateExceptions(bool indented)
    {
        var logger = LoggerBuilder.With(indented ? ConfigActions.Indented : ConfigActions.NoOp).Build();
        var leaf1 = new ArgumentException("leaf1").EnsureStackTrace();
        var leaf2 = new ArgumentException("leaf2").EnsureStackTrace();
        var leaf3 = new InvalidCastException("leaf3").EnsureStackTrace();
        var rootException = new AggregateException("root", leaf1, leaf2, leaf3).EnsureStackTrace();

        // Act
        logger.LogError(rootException, null);

        // Assert
        // Adaptions: only checks if exceptions are present via type and message

        logger.Formatted.Should().BeValidJson();
        var message = logger.Formatted!;

        message.Should().Contain(rootException.GetType().FullName).And.Contain(rootException.Message);
        message.Should().Contain(leaf1.GetType().FullName).And.Contain(leaf1.Message);
        message.Should().Contain(leaf2.GetType().FullName).And.Contain(leaf2.Message);
        message.Should().Contain(leaf3.GetType().FullName).And.Contain(leaf3.Message);

        // Assert.Contains(rootException.Message, json);
        // rootException.InnerExceptions.ToList().ForEach((inner) => Assert.Contains(inner.Message, json));
        //
        // Assert.Contains(GetContent(rootException), json);
        // rootException.InnerExceptions.ToList().ForEach((inner) => Assert.Contains(GetContent(inner), json));
    }
}
