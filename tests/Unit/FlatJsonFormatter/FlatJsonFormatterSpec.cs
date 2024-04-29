using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonFormatterSpec : SpecBase<FlatJsonConsoleFormatterOptions>
{
    public override bool OutputsOriginalFormat { get; } = false;
    public override bool ScopeOutputsMessage { get; } = false;
    public override bool ScopeOutputsOriginalFormat { get; } = false;

    public override FakeLoggerBuilder<FlatJsonConsoleFormatterOptions> CreateLoggerBuilder() => new(
        (optionsMonitor, _) =>
            new FlatJsonConsoleFormatter.FlatJsonConsoleFormatter(optionsMonitor), [ConfigActions.DontIncludeScopes]);
}
