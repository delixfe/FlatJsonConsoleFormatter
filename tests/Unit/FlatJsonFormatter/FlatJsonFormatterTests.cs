using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonFormatterTests : FormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
    FlatJsonConsoleFormatterOptions>
{
    public FlatJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
