using FlatJsonConsoleFormatter;
using Newtonsoft.Json.Linq;
using Unit.Suite;

namespace Unit.FlatJsonFormatter;

public class FlatJsonValidJsonFormatterTests :
    ValidJsonFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions,
        FlatJsonFormatterSpec>
{
    public FlatJsonValidJsonFormatterTests(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonMsJsonConsoleFormatterTests :
    MSJsonConsoleFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions,
        FlatJsonFormatterSpec>
{
    public FlatJsonMsJsonConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonMsConsoleFormatterTests :
    MSConsoleFormatterTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions,
        FlatJsonFormatterSpec>
{
    public FlatJsonMsConsoleFormatterTests(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }
}

public class FlatJsonStateOrScopePropertyTests_MergeDuplicateKeys_False :
    StateOrScopePropertyTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions,
        FlatJsonFormatterSpec>
{
    public FlatJsonStateOrScopePropertyTests_MergeDuplicateKeys_False(ITestOutputHelper testOutputHelper) : base(
        new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }

    protected override void PropertyIsNotOverwrittenAdditionalAssertions(string key, string elementName,
        PropertyNameDuplication duplication, string message)
    {
        var jToken = JToken.Parse(message);

        jToken.Should().HaveElement($"{elementName}_1");

        if (duplication == PropertyNameDuplication.Both)
            jToken.Should().HaveElement($"{elementName}_2");
    }

    protected override void CustomizeLoggerBuilder(FakeLoggerBuilder<FlatJsonConsoleFormatterOptions> builder) =>
        builder.With(o => o.MergeDuplicateKeys = false);
}

// MergeDuplicateKeys_True will overwrite properties with the same key
#if FALSE
public class FlatJsonStateOrScopePropertyTests_MergeDuplicateKeys_True :
    StateOrScopePropertyTests<FlatJsonConsoleFormatter.FlatJsonConsoleFormatter, FlatJsonConsoleFormatterOptions, FlatJsonFormatterSpec>
{
    public FlatJsonStateOrScopePropertyTests_MergeDuplicateKeys_True(ITestOutputHelper testOutputHelper) : base(new FlatJsonFormatterSpec(),
        testOutputHelper)
    {
    }

    protected override void CustomizeLoggerBuilder(FakeLoggerBuilder<FlatJsonConsoleFormatterOptions> builder)
    {
        builder.With(o => o.MergeDuplicateKeys = true);
    }
}
#endif
