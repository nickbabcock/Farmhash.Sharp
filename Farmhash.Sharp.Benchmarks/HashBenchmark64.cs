using BenchmarkDotNet.Attributes;
using System.Text;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Columns;

#if !CORE
using System.Data.HashFunction;
using xxHashSharp;
#endif

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

#if !CORE
        private static readonly SpookyHashV2 Spooky64 = new SpookyHashV2(64);
        private static readonly System.Data.HashFunction.CityHash hcity64 = new System.Data.HashFunction.CityHash(64);

        [Benchmark]
        public uint XXHash() =>  xxHash.CalculateHash(data);

        [Benchmark]
        public ulong CityHashNet() => CityHash.CityHash.CityHash64(dataStr);

        [Benchmark]
        public byte[] HashFunctionCityHash() => hcity64.ComputeHash(data);

        [Benchmark]
        public byte[] SpookyHash() => Spooky64.ComputeHash(data);

        [Benchmark]
        public unsafe ulong Spookily()
        {
            fixed (byte* buffer = data)
            {
                return SpookilySharp.SpookyHash.Hash64(buffer, data.Length, 0);
            }
        }
#endif
    }
}
