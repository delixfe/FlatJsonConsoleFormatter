using FlatJsonConsoleFormatter;

namespace Unit.FlatJsonFormatter;

public class FlatJsonFormatterTests : FormatterTestsBase<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter,
    FlatJsonConsoleFormatterOptions, FlatJsonFormatterSpec>
{
    public FlatJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}
