using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using Benchmarks.Infrastructure;
using Benchmarks.Scenarios;
using Microsoft.Extensions.Logging;

namespace Benchmarks;

public class Types
{
    private ILogger? _flatJsonLogger;
    private ILogger? _flatJsonLoggerUnsafeRelaxedJsonEscaping;

    private ILogger? _jsonLogger;
    private ILogger? _jsonLoggerUnsafeRelaxedJsonEscaping;

    [GlobalSetup]
    public void Setup()
    {
        _jsonLogger = Builder.CreateJsonLogger();
        _jsonLoggerUnsafeRelaxedJsonEscaping = Builder.CreateJsonLogger(Builder.UnsafeRelaxedJsonEscaping);
        _flatJsonLogger = Builder.CreateFlatJsonLogger();
        _flatJsonLoggerUnsafeRelaxedJsonEscaping = Builder.CreateFlatJsonLogger(Builder.UnsafeRelaxedJsonEscaping);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Run(ILogger logger) => TypeBased.ForEachDefinition(logger);


    [Benchmark(Baseline = true, OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void Json() => Run(_jsonLogger!);

    [Benchmark(OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void Json_UnsafeJson() => Run(_jsonLoggerUnsafeRelaxedJsonEscaping!);

    [Benchmark(OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void FlatJson() => Run(_flatJsonLogger!);

    [Benchmark(OperationsPerInvoke = TypeBased.DefinitionCount)]
    public void FlatJson_UnsafeJson() => Run(_flatJsonLoggerUnsafeRelaxedJsonEscaping!);
}
