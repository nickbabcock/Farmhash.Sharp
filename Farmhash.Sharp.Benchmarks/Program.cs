using System.Linq;
using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;

namespace Farmhash.Sharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var runFarmhashOnly = args.Contains("--farmhash");

            var config32 = new Config32();
            var config64 = new Config64();
            if (runFarmhashOnly)
            {
                config32.Add(new NameFilter(name => name.Contains("FarmHash")));
                config64.Add(new NameFilter(name => name.Contains("FarmHash")));
            }

            BenchmarkRunner.Run<HashBenchmark32>(config32);
            BenchmarkRunner.Run<HashBenchmark64>(config64);
        }
    }
}