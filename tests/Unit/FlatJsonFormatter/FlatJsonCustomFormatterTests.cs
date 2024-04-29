using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonCustomFormatterTests : FormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
    FlatJsonConsoleFormatterOptions>
{
    public FlatJsonCustomFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
