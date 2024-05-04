using FlatJsonConsoleFormatter;
using Unit.Suite;

namespace Unit.FlatJsonFormatter;

public class FlatJsonValidJsonFormatterTests :
    ValidJsonFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions>
{
    public FlatJsonValidJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonMsJsonConsoleFormatterTests :
    MSJsonConsoleFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
        FlatJsonConsoleFormatterOptions>
{
    public FlatJsonMsJsonConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonMsConsoleFormatterTests :
    MSConsoleFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
        FlatJsonConsoleFormatterOptions>
{
    public FlatJsonMsConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonStateOrScopePropertyTests :
    StateOrScopePropertyTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions>
{
    public FlatJsonStateOrScopePropertyTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }

    [Theory(Skip = "should be fixed or described as edge case")]
    [CombinatorialData]
    public override void
        DuplicatePropertyNames_NeverOverwritesStandardProperties(PropertyNameDuplicateHandling duplicateHandling) =>
        base.DuplicatePropertyNames_NeverOverwritesStandardProperties(duplicateHandling);
}
