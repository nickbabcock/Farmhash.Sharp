using BenchmarkDotNet.Attributes;
using System;
using System.Data.HashFunction;
using System.Security.Cryptography;
using System.Text;
using xxHashSharp;

namespace Farmhash.Sharp.Benchmarks
{
    public class HashBenchmark64
    {
        private static readonly MD5 md5 = MD5.Create();
        private static readonly SpookyHashV2 Spooky64 = new SpookyHashV2(64);
        private static readonly System.Data.HashFunction.CityHash hcity64 = new System.Data.HashFunction.CityHash(64);

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
        public byte[] Md5() => md5.ComputeHash(data);

        [Benchmark]
        public ulong FarmHash() => Farmhash.Hash64(data, data.Length);

        [Benchmark]
        public uint XXHash() =>  xxHash.CalculateHash(data);

        [Benchmark]
        public ulong CityHashNet() => CityHash.CityHash.CityHash64(dataStr);

        [Benchmark]
        public byte[] HashFunctionCityHash() => hcity64.ComputeHash(data);

        [Benchmark]
        public byte[] SpookyHash() => Spooky64.ComputeHash(data);
    }
}
