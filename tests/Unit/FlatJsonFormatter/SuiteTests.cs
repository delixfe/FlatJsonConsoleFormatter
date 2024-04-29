using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonValidJsonFormatterTests :
    ValidJsonFormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions>
{
    public FlatJsonValidJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonExtensionsLoggingJsonConsoleFormatterTests :
    ExtensionsLoggingJsonConsoleFormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
        FlatJsonConsoleFormatterOptions>
{
    public FlatJsonExtensionsLoggingJsonConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonExtensionsLoggingConsoleFormatterTests :
    ExtensionsLoggingConsoleFormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
        FlatJsonConsoleFormatterOptions>
{
    public FlatJsonExtensionsLoggingConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonStateOrScopePropertyTests :
    StateOrScopePropertyTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions>
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
