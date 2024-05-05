using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class FormatterTestsBase<TFormatter, TFormatterOptions, TSpec>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
    where TSpec : SpecBase<TFormatterOptions>
{
    protected const string _state = "This is a test, and {curly braces} are just fine!";
#pragma warning disable CS8603 // Possible null reference return
    protected readonly Func<object, Exception?, string> _defaultFormatter = (state, _) => state?.ToString();
#pragma warning restore CS8603 // Possible null reference return.

    protected FormatterTestsBase(TSpec spec,
        ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        Spec = spec;
    }

    public TSpec Spec { get; }

    public ITestOutputHelper TestOutputHelper { get; }

    public FakeLoggerBuilder<TFormatterOptions> LoggerBuilder
    {
        get
        {
            var builder = Spec.CreateLoggerBuilder().WithTestOutputHelper(TestOutputHelper);
            CustomizeLoggerBuilder(builder);
            return builder;
        }
    }

    protected virtual void CustomizeLoggerBuilder(FakeLoggerBuilder<TFormatterOptions> builder)
    {
        // no-op
    }


    public string JsonKeyValue<T>(string key, T value) =>
        Spec.CreateJsonKeyValuePair(key, value);

    public string MappedKey(string key) =>
        Spec.MapStateOrScopeElementNames(key);
}
