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

public class FlatJsonScopeFormatterTests :
    ScopeFormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions>
{
    public FlatJsonScopeFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
