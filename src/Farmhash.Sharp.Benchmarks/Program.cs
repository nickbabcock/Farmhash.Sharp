using System;
using System.Data.HashFunction;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using xxHashSharp;

namespace Farmhash.Sharp.Benchmarks
{
    class Program
    {
        private static byte[] StrToBytes(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        private const string Smallest = "doge";
        private const string Smaller = "hello world";
        private const string Small = "Hello I'm Farmhash.Sharp";
        private static readonly string Medium = new string('.', 100);
        private static readonly string Large = new string('.', 1000);
        private static readonly string Largest = new string('.', 10000);

        private static readonly MD5 md5 = MD5.Create();
        private static readonly System.Data.HashFunction.CityHash hcity = new System.Data.HashFunction.CityHash(32);
        private static readonly System.Data.HashFunction.CityHash hcity64 = new System.Data.HashFunction.CityHash(64);
        private static readonly SpookyHashV2 Spooky = new SpookyHashV2(32);
        private static readonly SpookyHashV2 Spooky64 = new SpookyHashV2(64);

        static void Main()
        {
            Console.WriteLine("Iterations per millisecond 32bit");
            Console.WriteLine("Name\tSmallest\tSmaller\tSmall\tMedium\tLarge\tLargest");
            ProfileSuite("Farmhash", 10000, bytes => Farmhash.Hash32(bytes, bytes.Length));
            ProfileSuite("xxHashSharp", 10000, bytes => xxHash.CalculateHash(bytes));
            ProfileSuite("CityHash.Net", 1000, str => CityHash.CityHash.CityHash32(str));
            ProfileSuite("String hashCode", 10000, (string str) => str.GetHashCode());
            ProfileSuite("HashFunction CityHash", 10000, (byte[] bytes) => hcity.ComputeHash(bytes));
            ProfileSuite("HashFunction Spooky", 1000, (byte[] bytes) => Spooky.ComputeHash(bytes));
            ProfileSuite("MD5sum", 100, bytes => md5.ComputeHash(bytes));

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Iterations per millisecond 64bit");
            Console.WriteLine("Name\tSmallest\tSmaller\tSmall\tMedium\tLarge\tLargest");
            ProfileSuite("Farmhash", 10000, bytes => Farmhash.Hash64(bytes, bytes.LongLength));
            ProfileSuite("xxHashSharp", 10000, bytes => xxHash.CalculateHash(bytes));
            ProfileSuite("CityHash.Net", 1000, str => CityHash.CityHash.CityHash64(str));
            ProfileSuite("HashFunction CityHash", 10000, (byte[] bytes) => hcity64.ComputeHash(bytes));
            ProfileSuite("HashFunction Spooky", 1000, (byte[] bytes) => Spooky64.ComputeHash(bytes));
            ProfileSuite("MD5sum", 100, bytes => md5.ComputeHash(bytes));
        }

        private static void ProfileSuite(string description, int iterations, Action<byte[]> calc)
        {
            double smallest, smaller, small, medium, large, largest;
            {
                byte[] bytes = StrToBytes(Smallest);
                smallest = Profile(iterations, () => calc(bytes));
            }

            {
                byte[] bytes = StrToBytes(Smaller);
                smaller = Profile(iterations, () => calc(bytes));
            }

            {
                byte[] bytes = StrToBytes(Small);
                small = Profile(iterations, () => calc(bytes));
            }

            {
                byte[] bytes = StrToBytes(Medium);
                medium = Profile(iterations, () => calc(bytes));
            }

            {
                byte[] bytes = StrToBytes(Large);
                large = Profile(iterations, () => calc(bytes));
            }

            {
                byte[] bytes = StrToBytes(Largest);
                largest = Profile(iterations, () => calc(bytes));
            }

            Console.WriteLine("{0}\t{1:N}\t{2:N}\t{3:N}\t{4:N}\t{5:N}\t{6:N}", description, smallest, smaller, small, medium, large, largest);
        }

        private static void ProfileSuite(string description, int iterations, Action<string> calc)
        {
            var smallest = Profile(iterations, () => calc(Smallest));
            var smaller = Profile(iterations, () => calc(Smaller));
            var small = Profile(iterations, () => calc(Small));
            var medium = Profile(iterations, () => calc(Medium));
            var large = Profile(iterations, () => calc(Large));
            var largest = Profile(iterations, () => calc(Largest));

            Console.WriteLine("{0}\t{1:N}\t{2:N}\t{3:N}\t{4:N}\t{5:N}\t{6:N}", description, smallest, smaller, small, medium, large, largest);
        }

        private static double Profile(int iterations, Action func)
        {
            func();
            func();
            func();
            func();
            func();

            // clean up
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var watch = Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++)
            {
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
                func();
            }
            watch.Stop();
            return iterations*15 / watch.Elapsed.TotalMilliseconds;
        }
    }
}
