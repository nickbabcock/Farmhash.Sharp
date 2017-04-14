using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System.Text;

namespace Farmhash.Sharp.Benchmarks
{
    [ClrJob, CoreJob, MonoJob]
    [LegacyJitX86Job, LegacyJitX64Job, RyuJitX64Job]
    public class HashBenchmark32
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
        public uint FarmHash() => Farmhash.Hash32(data, data.Length);
    }
}
