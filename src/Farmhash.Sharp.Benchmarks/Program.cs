using BenchmarkDotNet.Running;

namespace Farmhash.Sharp.Benchmarks
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<HashBenchmark32>();
            BenchmarkRunner.Run<HashBenchmark64>();
        }
    }
}
