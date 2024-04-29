using JsonConsoleFormatters;

namespace Unit.JeapJsonFormatter;

public class JeapJsonValidJsonFormatterTests :
    ValidJsonFormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonValidJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonExtensionsLoggingJsonConsoleFormatterTests :
    ExtensionsLoggingJsonConsoleFormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonExtensionsLoggingJsonConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonExtensionsLoggingConsoleFormatterTest :
    ExtensionsLoggingConsoleFormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonExtensionsLoggingConsoleFormatterTest(ITestOutputHelper testOutputHelper) : base(
        new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonStateOrScopePropertyTests :
    StateOrScopePropertyTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonStateOrScopePropertyTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
