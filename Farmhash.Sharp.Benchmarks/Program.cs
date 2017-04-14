using BenchmarkDotNet.Running;

namespace Farmhash.Sharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<HashBenchmark32>();
        }
    }
}