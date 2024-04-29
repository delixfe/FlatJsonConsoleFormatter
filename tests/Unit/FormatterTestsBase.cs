using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class FormatterTestsBase<TFormatter, TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected const string _state = "This is a test, and {curly braces} are just fine!";
#pragma warning disable CS8603 // Possible null reference return
    protected readonly Func<object, Exception?, string> _defaultFormatter = (state, _) => state?.ToString();
#pragma warning restore CS8603 // Possible null reference return.

    protected FormatterTestsBase(SpecBase<TFormatterOptions> spec,
        ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        Spec = spec;
        LoggerBuilder = spec.CreateLoggerBuilder();
    }

    public SpecBase<TFormatterOptions> Spec { get; }

    public ITestOutputHelper TestOutputHelper { get; }
    public FakeLoggerBuilder<TFormatterOptions> LoggerBuilder { get; }
}
