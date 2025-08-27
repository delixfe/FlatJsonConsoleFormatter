using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Benchmarks.Infrastructure;

public class UnrollFactorAttribute : JobMutatorConfigBaseAttribute
{
    public UnrollFactorAttribute(int unrollFactor = 16) : base(Job.Default.WithUnrollFactor(unrollFactor))
    {
    }
}
