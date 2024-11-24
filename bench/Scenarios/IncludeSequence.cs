using BenchmarkDotNet.Attributes;
using Benchmarks.Infrastructure;
using Microsoft.Extensions.Logging;
using Proto.Utilities.Benchmark;

namespace Benchmarks.Scenarios;

[BenchmarkCategory(Categories.IncludeSequence)]
[RunOncePerIteration]
public partial class IncludeSequence
{
    private const int OperationsPerInvoke = 1024 * 1024;
    private BenchmarkThreadHelper? _threadHelper;

    [GlobalSetup(Target = nameof(Jeap_DontIncludeSequence))]
    public void Jeap_DontIncludeSequence_Setup()
    {
        var logger =
            Builder.CreateJeapJsonLogger(Builder.DontIncludeScopes, Builder.DontIncludeSequence);
        _threadHelper = CreateThreadHelperWithActionsFor(logger);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = OperationsPerInvoke)]
    public void Jeap_DontIncludeSequence() => _threadHelper!.ExecuteAndWait();

    [GlobalSetup(Target = nameof(Jeap_IncludeSequence))]
    public void Jeap_IncludeSequence_Setup()
    {
        var logger =
            Builder.CreateJeapJsonLogger(Builder.DontIncludeScopes, Builder.IncludeSequence);
        _threadHelper = CreateThreadHelperWithActionsFor(logger);
    }

    [Benchmark(OperationsPerInvoke = OperationsPerInvoke)]
    public void Jeap_IncludeSequence() => _threadHelper!.ExecuteAndWait();

    private static BenchmarkThreadHelper CreateThreadHelperWithActionsFor(ILogger logger)
    {
        var helper = new BenchmarkThreadHelper();
        var action = () => LogSomething(logger);

        for (var i = 0; i < OperationsPerInvoke; i++)
        {
            helper.Add(action);
        }

        return helper;
    }

    [LoggerMessage(LogLevel.Information, Message = "42")]
    private static partial void LogSomething(ILogger logger);

    [GlobalCleanup]
    public void Cleanup() => _threadHelper?.Dispose();
}
