using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Farmhash.Sharp
{
    static class Simd
    {
        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L209-L212
        private static uint Rotate32(uint val, int shift) =>
            shift == 0 ? val : (val >> shift) | (val << (32 - shift));

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L214-L217
        private static long Rotate64(long val, int shift) =>
            shift == 0 ? val : (val >> shift) | (val << (64 - shift));

        private static long Rotate(long val, int shift) => Rotate64(val, shift);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Add(Vector128<int> x, Vector128<int> y) => Sse42.Add(x, y);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Xor(Vector128<int> x, Vector128<int> y) => Sse42.Xor(x, y);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Mul(Vector128<int> x, Vector128<int> y) => Sse42.MultiplyLow(x, y);
        
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<int> Shuf(Vector128<int> x, Vector128<int> y) => Sse42.Shuffle(y.AsByte(), x.AsByte()).AsInt32();

        public static void swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static unsafe ulong Hash64Long(byte* s, uint len, long seed0, long seed1)
        {
            Vector128<int> kShuf = Vector128.Create(4, 11, 10, 5, 8, 15, 6, 9, 12, 2, 14, 13, 0, 7, 3, 1).AsInt32();
            var kMult = Vector128.Create(0xbd, 0xd6, 0x33, 0x39, 0x45, 0x54, 0xfa, 0x03, 0x34, 0x3e, 0x33, 0xed, 0xcc, 0x9e, 0x2d, 0x51).AsInt32();
            long seed2 = (seed0 + 113) * (seed1 + 9);
            long seed3 = (Rotate(seed0, 23) + 27) * (Rotate(seed1, 30) + 111);

            var d0 = Sse42.X64.ConvertScalarToVector128Int64(seed0).AsInt32();
            var d1 = Sse42.X64.ConvertScalarToVector128Int64(seed1).AsInt32();
            var d2 = Shuf(kShuf, d0);
            var d3 = Shuf(kShuf, d1);
            var d4 = Xor(d0, d1);
            var d5 = Xor(d1, d2);
            var d6 = Xor(d2, d4);
            var d7 = Vector128.Create((int)(seed2 >> 32));
            var d8 = Mul(kMult, d2);
            var d9 = Vector128.Create((int)(seed3 >> 32));
            var d10 = Vector128.Create((int)seed3);
            var d11 = Add(d2, Vector128.Create((int)seed2));

            var end = s + (len & ~255);
            do
            {
                var z = Sse42.LoadVector128(s).AsInt32();
                d0 = Add(d0, z);
                d1 = Shuf(kShuf, d1);
                d2 = Xor(d2, d0);
                d4 = Xor(d4, z);
                d4 = Xor(d4, d1);
                swap(ref d0, ref d6);
                z = Sse42.LoadVector128(s + 16).AsInt32();
                d5 = Add(d5, z);
                d6 = Shuf(kShuf, d6);
                d8 = Shuf(kShuf, d8);
                d7 = Xor(d7, d5);
                d0 = Xor(d0, z);
                d0 = Xor(d0, d6);
                swap(ref d5, ref d11);
                z = Sse42.LoadVector128(s + 32).AsInt32();
                d1 = Add(d1, z);
                d2 = Shuf(kShuf, d2);
                d4 = Shuf(kShuf, d4);
                d5 = Xor(d5, z);
                d5 = Xor(d5, d2);
                swap(ref d10, ref d4);
                z = Sse42.LoadVector128(s + 48).AsInt32();
                d6 = Add(d6, z);
                d7 = Shuf(kShuf, d7);
                d0 = Shuf(kShuf, d0);
                d8 = Xor(d8, d6);
                d1 = Xor(d1, z);
                d1 = Add(d1, d7);
                z = Sse42.LoadVector128(s + 64).AsInt32();
                d2 = Add(d2, z);
                d5 = Shuf(kShuf, d5);
                d4 = Add(d4, d2);
                d6 = Xor(d6, z);
                d6 = Xor(d6, d11);
                swap(ref d8, ref d2);
                z = Sse42.LoadVector128(s + 80).AsInt32();
                d7 = Xor(d7, z);
                d8 = Shuf(kShuf, d8);
                d1 = Shuf(kShuf, d1);
                d0 = Add(d0, d7);
                d2 = Add(d2, z);
                d2 = Add(d2, d8);
                swap(ref d1, ref d7);
                z = Sse42.LoadVector128(s + 96).AsInt32();
                d4 = Shuf(kShuf, d4);
                d6 = Shuf(kShuf, d6);
                d8 = Mul(kMult, d8);
                d5 = Xor(d5, d11);
                d7 = Xor(d7, z);
                d7 = Add(d7, d4);
                swap(ref d6, ref d0);
                z = Sse42.LoadVector128(s + 112).AsInt32();
                d8 = Add(d8, z);
                d0 = Shuf(kShuf, d0);
                d2 = Shuf(kShuf, d2);
                d1 = Xor(d1, d8);
                d10 = Xor(d10, z);
                d10 = Xor(d10, d0);
                swap(ref d11, ref d5);
                z = Sse42.LoadVector128(s + 128).AsInt32();
                d4 = Add(d4, z);
                d5 = Shuf(kShuf, d5);
                d7 = Shuf(kShuf, d7);
                d6 = Add(d6, d4);
                d8 = Xor(d8, z);
                d8 = Xor(d8, d5);
                swap(ref d4, ref d10);
                z = Sse42.LoadVector128(s + 144).AsInt32();
                d0 = Add(d0, z);
                d1 = Shuf(kShuf, d1);
                d2 = Add(d2, d0);
                d4 = Xor(d4, z);
                d4 = Xor(d4, d1);
                z = Sse42.LoadVector128(s + 160).AsInt32();
                d5 = Add(d5, z);
                d6 = Shuf(kShuf, d6);
                d8 = Shuf(kShuf, d8);
                d7 = Xor(d7, d5);
                d0 = Xor(d0, z);
                d0 = Xor(d0, d6);
                swap(ref d2, ref d8);
                z = Sse42.LoadVector128(s + 176).AsInt32();
                d1 = Add(d1, z);
                d2 = Shuf(kShuf, d2);
                d4 = Shuf(kShuf, d4);
                d5 = Mul(kMult, d5);
                d5 = Xor(d5, z);
                d5 = Xor(d5, d2);
                swap(ref d7, ref d1);
                z = Sse42.LoadVector128(s + 192).AsInt32();
                d6 = Add(d6, z);
                d7 = Shuf(kShuf, d7);
                d0 = Shuf(kShuf, d0);
                d8 = Add(d8, d6);
                d1 = Xor(d1, z);
                d1 = Xor(d1, d7);
                swap(ref d0, ref d6);
                z = Sse42.LoadVector128(s + 208).AsInt32();
                d2 = Add(d2, z);
                d5 = Shuf(kShuf, d5);
                d4 = Xor(d4, d2);
                d6 = Xor(d6, z);
                d6 = Xor(d6, d9);
                swap(ref d5, ref d11);
                z = Sse42.LoadVector128(s + 224).AsInt32();
                d7 = Add(d7, z);
                d8 = Shuf(kShuf, d8);
                d1 = Shuf(kShuf, d1);
                d0 = Xor(d0, d7);
                d2 = Xor(d2, z);
                d2 = Xor(d2, d8);
                swap(ref d10, ref d4);
                z = Sse42.LoadVector128(s + 240).AsInt32();
                d3 = Add(d3, z);
                d4 = Shuf(kShuf, d4);
                d6 = Shuf(kShuf, d6);
                d7 = Mul(kMult, d7);
                d5 = Add(d5, d3);
                d7 = Xor(d7, z);
                d7 = Xor(d7, d4);
                swap(ref d3, ref d9);
                s += 256;
            } while (s != end);

            d6 = Add(Mul(kMult, d6), Sse42.X64.ConvertScalarToVector128Int64(len).AsInt32());
            if (len % 256 != 0)
            {
                throw new NotImplementedException();
                //d7 = Add(_mm_shuffle_epi32(d8, (0 << 6) + (3 << 4) + (2 << 2) + (1 << 0)), d7);
                //d8 = Add(Mul(kMult, d8), _mm_cvtsi64_si128(farmhashxo::Hash64(s, len % 256)));
            }

            d0 = Mul(kMult, Shuf(kShuf, Mul(kMult, d0)));
            d3 = Mul(kMult, Shuf(kShuf, Mul(kMult, d3)));
            d9 = Mul(kMult, Shuf(kShuf, Mul(kMult, d9)));
            d1 = Mul(kMult, Shuf(kShuf, Mul(kMult, d1)));
            d0 = Add(d11, d0);
            d3 = Xor(d7, d3);
            d9 = Add(d8, d9);
            d1 = Add(d10, d1);
            d4 = Add(d3, d4);
            d5 = Add(d9, d5);
            d6 = Xor(d1, d6);
            d2 = Add(d0, d2);
            var t = new[]{ d0, d3, d9, d1, d4, d5, d6, d2 };
            fixed (Vector128<int>* buf = t)
            {
                return Farmhash.Hash64((byte*)buf, sizeof(Vector128<int>) * t.Length);
            }
            
            //t[0] = d0;
            //t[1] = d3;
            //t[2] = d9;
            //t[3] = d1;
            //t[4] = d4;
            //t[5] = d5;
            //t[6] = d6;
            //t[7] = d2;
            //return farmhashxo::Hash64(reinterpret_cast <const char*> (t), sizeof(t));
        }
    }
}
