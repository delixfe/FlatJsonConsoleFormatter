using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonFormatterSpec : SpecBase<FlatJsonConsoleFormatterOptions>
{
    public override FakeLoggerBuilder<FlatJsonConsoleFormatterOptions> CreateLoggerBuilder() => new(
        (optionsMonitor, _) =>
            new FlatJsonConsoleFormatter.FlatJsonConsoleFormatter(optionsMonitor), [ConfigActions.DontIncludeScopes]);
}
