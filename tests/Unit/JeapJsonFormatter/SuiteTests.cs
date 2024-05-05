using JsonConsoleFormatters;
using Newtonsoft.Json.Linq;
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

    protected override void PropertyIsNotOverwrittenAdditionalAssertions(string key, string elementName,
        PropertyNameDuplication duplication, string message)
    {
        var jToken = JToken.Parse(message);

        jToken.Should().HaveElement($"{elementName}_1");

        if (duplication == PropertyNameDuplication.Both)
            jToken.Should().HaveElement($"{elementName}_2");
    }

    [Theory]
    [CombinatorialData]
    public void Reserved_EventName(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten("EventName", "eventName", duplication, addEventId: true);

    [Theory]
    [CombinatorialData]
    public void Reserved_ThreadName(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten("Thread_name", "thread_name", duplication,
            customizeBuilder: builder => builder.With(o => o.IncludeThreadName = true));

    [Theory]
    [CombinatorialData]
    public void Reserved_Severity(PropertyNameDuplication duplication) =>
        PropertyIsNotOverwritten("Severity", "severity", duplication);
}
