using JsonConsoleFormatters;
using Unit.Infrastructure;
using Xunit.Abstractions;

namespace Unit;

public class JeapJsonFormatterTests : FormatterTestsBase<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>
{
    public JeapJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(FakeLoggerBuilder.JeapJson(), testOutputHelper)
    {
    }
}
