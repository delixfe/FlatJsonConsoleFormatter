using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Benchmarks.Infrastructure;
using Benchmarks.Scenarios;
using Microsoft.Extensions.Logging;

namespace Benchmarks;

public class Types
{
    private ILogger? _flatJsonLogger;
    private ILogger? _jeapJsonLogger;
    private ILogger? _jsonLogger;

    [GlobalSetup]
    public void Setup()
    {
        _jsonLogger = Builder.CreateJsonLogger();
        _jeapJsonLogger = Builder.CreateJeapJsonLogger();
        _flatJsonLogger = Builder.CreateFlatJsonLogger();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Run(ILogger logger) => TypeBased.ForEachDefinition(logger);

    [Benchmark(Baseline = true, OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void Json() => Run(_jsonLogger!);

    [Benchmark(OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void JeapJson() => Run(_jeapJsonLogger!);

    [Benchmark(OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void FlatJson() => Run(_flatJsonLogger!);
}
