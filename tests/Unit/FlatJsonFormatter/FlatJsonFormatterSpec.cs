using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonFormatterSpec : SpecBase<FlatJsonConsoleFormatterOptions>
{
    public override bool OutputsOriginalFormat { get; } = false;
    public override bool ScopeOutputsMessage { get; } = false;
    public override bool ScopeOutputsOriginalFormat { get; } = false;

    public override FakeLoggerBuilder<FlatJsonConsoleFormatterOptions> CreateLoggerBuilder() => new(
        (optionsMonitor, _) =>
            new FlatJsonConsoleFormatter.FlatJsonConsoleFormatter(optionsMonitor),
        Array.Empty<Action<FlatJsonConsoleFormatterOptions>>());

    public override Action<FlatJsonConsoleFormatterOptions> ConfigurePropertyNameDuplicateHandling(
        PropertyNameDuplicateHandling duplicateHandling) => duplicateHandling switch
    {
        PropertyNameDuplicateHandling.Overwrite => o => o.MergeDuplicateKeys = false,
        PropertyNameDuplicateHandling.UnderscoreIntSuffix => o => o.MergeDuplicateKeys = true,
        _ => throw new ArgumentOutOfRangeException(nameof(duplicateHandling), duplicateHandling, null)
    };

    public override Action<FlatJsonConsoleFormatterOptions> ConfigureIncludeEventHandling(bool includeEventHandling) =>
        o => o.IncludeEventId = includeEventHandling;
}
