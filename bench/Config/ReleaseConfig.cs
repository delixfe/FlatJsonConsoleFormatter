using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Benchmarks.Config;

public class ReleaseConfig : ManualConfig
{
    public ReleaseConfig()
    {
        AddDefaults();
        var defaultJob = Job.Default
            .WithMinIterationCount(15)
            .WithMaxIterationCount(100) // we don't want to run more than 100 iterations
            .DontEnforcePowerPlan() // make sure BDN does not try to enforce the High Performance power plan on Windows
            .WithGcServer(true)
            .WithUnrollFactor(
                1024 * 8); // we need to reach 100 ms of execution time; unrolling is the easiest way to do it

        AddJob(defaultJob.WithRuntime(CoreRuntime.Core80));
#if BENCH_NET9
        AddJob(defaultJob.WithRuntime(CoreRuntime.Core90));
#endif
        AddColumn(StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max,
            CategoriesColumn.Default);

        AddDiagnoser(MemoryDiagnoser.Default);

        AddExporter(MarkdownExporter.GitHub, JsonExporter.Full, HtmlExporter.Default);

        AddLogger(ConsoleLogger.Unicode);

        // ignore FlatJson benchmarks
        AddFilter(new NameFilter(name => !name.StartsWith("Flat")));

        WithOption(ConfigOptions.StopOnFirstError, true);

        SummaryStyle =
            SummaryStyle.Default
                .WithMaxParameterColumnWidth(36); // the default is 20 and trims some benchmark results too aggressively
    }

    private void AddDefaults()
    {
        AddColumnProvider(DefaultColumnProviders.Instance);
        AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
        AddValidator(DefaultConfig.Instance.GetValidators().ToArray());
    }
}
