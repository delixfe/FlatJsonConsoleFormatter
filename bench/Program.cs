using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Benchmarks.Config;
using Benchmarks.Infrastructure;

namespace Benchmarks;

internal class Program
{
    private static int Main(string[] args)
    {
        IConfig config;

#if DEBUG
        var debuggingConfig = new DebuggingConfig();
        config = debuggingConfig;
#else
        var releaseConfig = new ReleaseConfig();
        config = releaseConfig;
#endif

        return BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, config)
            .ToExitCode();
    }
}
