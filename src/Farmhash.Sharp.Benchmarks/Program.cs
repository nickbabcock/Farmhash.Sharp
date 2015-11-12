using System;
using System.Diagnostics;
using System.Text;

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
            var smallestFarm = Profile(10000, () => Farmhash.Hash32(Smallest, Smallest.Length));
            var smallerFarm = Profile(10000, () => Farmhash.Hash32(Smaller, Smaller.Length));
            var smallFarm = Profile(10000, () => Farmhash.Hash32(Small, Small.Length));
            var mediumFarm = Profile(10000, () => Farmhash.Hash32(Medium, Medium.Length));
            var largeFarm = Profile(10000, () => Farmhash.Hash32(Large, Large.Length));
            var largestFarm = Profile(10000, () => Farmhash.Hash32(Largest, Largest.Length));
            Console.WriteLine("Iterations per millisecond");
            Console.WriteLine("Smallest\tSmaller\tSmall\tMedium\tLarge\tLargest");
            Console.WriteLine("{0:N}\t{1:N}\t{2:N}\t{3:N}\t{4:N}\t{5:N}", smallestFarm, smallerFarm, smallFarm, mediumFarm, largeFarm, largestFarm);
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
