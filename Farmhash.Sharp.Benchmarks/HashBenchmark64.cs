using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System.Text;

namespace Farmhash.Sharp.Benchmarks
{
#if CORE
    [CoreJob]
#elif MONO
    [MonoJob]
#else
    [ClrJob]
    [LegacyJitX86Job, LegacyJitX64Job, RyuJitX64Job]
#endif
    public class HashBenchmark64
    {
        private byte[] data;
        private string dataStr;

        [Setup]
        public void SetupData()
        {
            dataStr = new string('.', PayloadLength);
            data = Encoding.ASCII.GetBytes(dataStr);
        }


        [Params(4, 11, 25, 100, 1000, 10000)]
        public int PayloadLength { get; set; }

        [Benchmark]
        public ulong FarmHash() => Farmhash.Hash64(data, data.Length);
    }
}
