using BenchmarkDotNet.Attributes;
using System;
using System.Data.HashFunction;
using System.Security.Cryptography;
using System.Text;
using xxHashSharp;

namespace Farmhash.Sharp.Benchmarks
{
    public class HashBenchmark32
    {
        private static readonly MD5 md5 = MD5.Create();
        private static readonly System.Data.HashFunction.CityHash hcity = new System.Data.HashFunction.CityHash(32);
        private static readonly SpookyHashV2 Spooky = new SpookyHashV2(32);

        private byte[] data;
        private string dataStr;

        [Setup]
        public void SetupData()
        {
            dataStr = new String('.', PayloadLength);
            data = Encoding.ASCII.GetBytes(dataStr);
        }

        [Params(4, 11, 25, 100, 1000, 10000)]
        public int PayloadLength { get; set; }

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);

        [Benchmark]
        public uint FarmHash() => Farmhash.Hash32(data, data.Length);

        [Benchmark]
        public uint XXHash() => xxHash.CalculateHash(data);

        [Benchmark]
        public uint CityHashNet() => CityHash.CityHash.CityHash32(dataStr);

        [Benchmark]
        public int StringHashCode() => dataStr.GetHashCode();

        [Benchmark]
        public byte[] HFCityHash() => hcity.ComputeHash(data);

        [Benchmark]
        public byte[] SpookyHash() => Spooky.ComputeHash(data);
    }
}
