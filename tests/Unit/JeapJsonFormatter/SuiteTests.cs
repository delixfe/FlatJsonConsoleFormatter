using JsonConsoleFormatters;
using Unit.Suite;

namespace Unit.JeapJsonFormatter;

public class JeapJsonValidJsonFormatterTests :
    ValidJsonFormatterTests<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonValidJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonMsJsonConsoleFormatterTests :
    MSJsonConsoleFormatterTests<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonMsJsonConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonMsConsoleFormatterTest :
    MSConsoleFormatterTests<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonMsConsoleFormatterTest(ITestOutputHelper testOutputHelper) : base(
        new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class JeapJsonStateOrScopePropertyTests :
    StateOrScopePropertyTests<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonStateOrScopePropertyTests(ITestOutputHelper testOutputHelper) : base(new JeapJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
