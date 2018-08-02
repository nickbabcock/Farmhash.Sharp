using System.Data.HashFunction.CityHash;
using System.Data.HashFunction.SpookyHash;
using BenchmarkDotNet.Attributes;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Columns;

namespace Farmhash.Sharp.Benchmarks
{
    public class Config64 : ManualConfig
    {
        public Config64()
        {
            Add(new BaseConfig());
            Add(new TagColumn("Kind", _ => "64bit hash"));
        }
    }

    [Config(typeof(Config64))]
    public class HashBenchmark64
    {
        private static readonly ICityHash City64 = CityHashFactory.Instance.Create(
            new CityHashConfig { HashSizeInBits = 64 });

        private static readonly ISpookyHash Spooky64 = SpookyHashV2Factory.Instance.Create(
            new SpookyHashConfig() { HashSizeInBits = 64});

        private byte[] data;
        private string dataStr;

        [GlobalSetup]
        public void SetupData()
        {
            dataStr = new string('.', PayloadLength);
            data = Encoding.ASCII.GetBytes(dataStr);
        }

        [Params(4, 11, 25, 100, 1000, 10000)]
        public int PayloadLength { get; set; }

        [Benchmark]
        public ulong FarmHash() => Farmhash.Hash64(data, data.Length);

        [Benchmark]
        public ulong SparrowXXHash() => SparrowHashing.XXHash64.Calculate(data, data.Length);

        [Benchmark]
        public byte[] HFCityHash() => City64.ComputeHash(data).Hash;

        [Benchmark]
        public byte[] SpookyHash() => Spooky64.ComputeHash(data).Hash;

        [Benchmark]
        public unsafe ulong Spookily()
        {
            fixed (byte* buffer = data)
            {
                return SpookilySharp.SpookyHash.Hash64(buffer, data.Length, 0);
            }
        }

#if NET461
        [Benchmark]
        public ulong CityHashNet() => CityHash.CityHash.CityHash64(dataStr);
#else
        [Benchmark]
        public ulong CityHashNet() => 0;
#endif
    }
}
