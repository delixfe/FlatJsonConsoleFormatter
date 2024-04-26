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
    private ILogger? _flatJsonLoggerUnsafeRelaxedJsonEscaping;
    
    private ILogger? _jeapLogger;
    private ILogger? _jeapLoggerUnsafeRelaxedJsonEscaping;

    private ILogger? _jsonLogger;
    private ILogger? _jsonLoggerUnsafeRelaxedJsonEscaping;

    [GlobalSetup]
    public void Setup()
    {
        _jsonLogger = Builder.CreateJsonLogger(Builder.IncludeScopes);
        _jsonLoggerUnsafeRelaxedJsonEscaping =
            Builder.CreateJsonLogger(Builder.IncludeScopes, Builder.UnsafeRelaxedJsonEscaping);
       
        _jeapLogger = Builder.CreateJeapJsonLogger(Builder.IncludeScopes);
        _jeapLoggerUnsafeRelaxedJsonEscaping =
            Builder.CreateJeapJsonLogger(Builder.IncludeScopes, Builder.UnsafeRelaxedJsonEscaping);
       
        _flatJsonLogger = Builder.CreateFlatJsonLogger(Builder.IncludeScopes);
        _flatJsonLoggerMergeDuplicateKeys =
            Builder.CreateFlatJsonLogger(Builder.IncludeScopes, Builder.MergeDuplKeys);
        _flatJsonLoggerUnsafeRelaxedJsonEscaping =
            Builder.CreateFlatJsonLogger(Builder.IncludeScopes, Builder.UnsafeRelaxedJsonEscaping);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void Run(ILogger logger) => AspNetScenario.NineAttributes(logger);
    
    [Benchmark(Baseline = true)]
    public void Json() => Run(_jsonLogger!);

    [Benchmark]
    public void Json_UnsafeJson() => Run(_jsonLoggerUnsafeRelaxedJsonEscaping!);
    
    [Benchmark]
    public void JeapJson() => Run(_jeapLogger!);
    
    [Benchmark]
    public void JeapJson_UnsafeJson() => Run(_jeapLoggerUnsafeRelaxedJsonEscaping!);

    [Benchmark]
    public void FlatJson() => Run(_flatJsonLogger!);

    [Benchmark]
    public void FlatJson_UnsafeJson() =>
        Run(_flatJsonLoggerUnsafeRelaxedJsonEscaping!);

    [Benchmark]
    public void FlatJson_MergeDuplKeys() => Run(_flatJsonLoggerMergeDuplicateKeys!);
}
