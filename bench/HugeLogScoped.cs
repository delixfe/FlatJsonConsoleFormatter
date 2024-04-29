using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Benchmarks.Infrastructure;
using Benchmarks.Scenarios;
using Microsoft.Extensions.Logging;

namespace Benchmarks;

[BenchmarkCategory(Categories.WithScope)]
public class HugeLogScoped
{
    private ILogger? _flatJsonLogger;
    private ILogger? _flatJsonLoggerMergeDuplicateKeys;

    private ILogger? _jeapLogger;

    private ILogger? _jsonLogger;

    [GlobalSetup]
    public void Setup()
    {
        _jsonLogger = Builder.CreateJsonLogger(Builder.IncludeScopes);

        _jeapLogger = Builder.CreateJeapJsonLogger(Builder.IncludeScopes);

        _flatJsonLogger = Builder.CreateFlatJsonLogger(Builder.IncludeScopes);
        _flatJsonLoggerMergeDuplicateKeys =
            Builder.CreateFlatJsonLogger(Builder.IncludeScopes, Builder.MergeDuplKeys);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Run(ILogger logger) => AspNetScenario.NineAttributes(logger);

    [Benchmark(Baseline = true)]
    public void Json() => Run(_jsonLogger!);

    [Benchmark]
    public void JeapJson() => Run(_jeapLogger!);

    [Benchmark]
    public void FlatJson() => Run(_flatJsonLogger!);

    [Benchmark]
    public void FlatJson_MergeDuplKeys() => Run(_flatJsonLoggerMergeDuplicateKeys!);
}
