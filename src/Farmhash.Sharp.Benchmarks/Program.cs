using System;
using System.Diagnostics;
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

        private static readonly byte[] Smallest = StrToBytes("doge");
        private static readonly byte[] Smaller = StrToBytes("hello world");
        private static readonly byte[] Small = StrToBytes("Hello I'm Farmhash.Sharp");
        private static readonly byte[] Medium = StrToBytes(new string('.', 100));
        private static readonly byte[] Large = StrToBytes(new string('.', 1000));
        private static readonly byte[] Largest = StrToBytes(new string('.', 10000));

        static void Main()
        {
            Console.WriteLine("Iterations per millisecond");
            Console.WriteLine("Name\tSmallest\tSmaller\tSmall\tMedium\tLarge\tLargest");
            ProfileSuite("Farmhash", 10000, bytes => Farmhash.Hash32(bytes, bytes.Length));
            ProfileSuite("xxHashSharp", 10000, bytes => xxHash.CalculateHash(bytes));
        }

        private static void ProfileSuite(string description, int iterations, Action<byte[]> calc)
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
