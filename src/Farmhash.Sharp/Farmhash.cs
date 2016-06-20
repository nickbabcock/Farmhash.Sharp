// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_BuiltInTypes
using System.Runtime.CompilerServices;
using System.Text;
namespace Farmhash.Sharp
{
    /// <summary>
    /// Class that can calculate 32bit and 64bit hashes using Google's farmhash algorithm
    /// </summary>
    public static class Farmhash
    {
        private struct uint128_t
        {
            public ulong first, second;
            public uint128_t(ulong first, ulong second)
            {
                this.first = first;
                this.second = second;
            }
        }

        // Some primes between 2^63 and 2^64 for various uses.
        private const ulong k0 = 0xc3a5c85c97cb3127U;
        private const ulong k1 = 0xb492b66fbe98f273U;
        private const ulong k2 = 0x9ae16a3b2f90404fU;

        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        private const uint c1 = 0xcc9e2d51;
        private const uint c2 = 0x1b873593;

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L209-L212
        private static uint Rotate32(uint val, int shift) =>
            shift == 0 ? val : (val >> shift) | (val << (32 - shift));

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L214-L217
        private static ulong Rotate64(ulong val, int shift) =>
            shift == 0 ? val : (val >> shift) | (val << (64 - shift));

        private static uint Rotate(uint val, int shift) => Rotate32(val, shift);

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L192-L196
        private static unsafe ulong Fetch64(byte* p) => *(ulong*) p;

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L198-L202
        private static unsafe uint Fetch32(byte* p) => *(uint*)p;

        private static unsafe uint Fetch(byte* p) => Fetch32(p);

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L360-L369
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L1042-L1051
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Hash32Len0to4(byte[] s, uint len, uint seed = 0)
        {
            uint b = seed;
            uint c = 9;
            for (int i = 0; i < len; i++)
            {
                b = b * c1 + s[i];
                c ^= b;
            }
            return fmix(Mur(b, Mur(len, c)));
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L371-L379
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Mur(uint a, uint h)
        {
            // Helper from Murmur3 for combining two 32-bit values.
            a *= c1;
            a = Rotate32(a, 17);
            a *= c2;
            h ^= a;
            h = Rotate32(h, 19);
            return h * 5 + 0xe6546b64;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L1025-L1040
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Hash32Len13to24(byte* s, uint len, uint seed = 0) {
            uint a = Fetch(s - 4 + (len >> 1));
            uint b = Fetch(s + 4);
            uint c = Fetch(s + len - 8);
            uint d = Fetch(s + (len >> 1));
            uint e = Fetch(s);
            uint f = Fetch(s + len - 4);
            uint h = d * c1 + len + seed;
            a = Rotate(a, 12) + f;
            h = Mur(c, h) + a;
            a = Rotate(a, 3) + c;
            h = Mur(e, h) + a;
            a = Rotate(a + f, 12) + d;
            h = Mur(b ^ seed, h) + a;
            return fmix(h);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L1053-L1059
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Hash32Len5to12(byte* s, uint len, uint seed = 0)
        {
            uint a = len, b = len * 5, c = 9, d = b + seed;
            a += Fetch(s);
            b += Fetch(s + len - 4);
            c += Fetch(s + ((len >> 1) & 4));
            return fmix(seed ^ Mur(c, Mur(b, Mur(a, d))));
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L1061-L1117
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint Hash32(byte* s, uint len)
        {
            if (len <= 24)
            {
                return len <= 12 ? Hash32Len5to12(s, len) :
                    Hash32Len13to24(s, len);
            }

            uint h = len, g = c1 * len, f = g;
            uint a0 = Rotate(Fetch(s + len - 4) * c1, 17) * c2;
            uint a1 = Rotate(Fetch(s + len - 8) * c1, 17) * c2;
            uint a2 = Rotate(Fetch(s + len - 16) * c1, 17) * c2;
            uint a3 = Rotate(Fetch(s + len - 12) * c1, 17) * c2;
            uint a4 = Rotate(Fetch(s + len - 20) * c1, 17) * c2;
            h ^= a0;
            h = Rotate(h, 19);
            h = h * 5 + 0xe6546b64;
            h ^= a2;
            h = Rotate(h, 19);
            h = h * 5 + 0xe6546b64;
            g ^= a1;
            g = Rotate(g, 19);
            g = g * 5 + 0xe6546b64;
            g ^= a3;
            g = Rotate(g, 19);
            g = g * 5 + 0xe6546b64;
            f += a4;
            f = Rotate(f, 19) + 113;
            uint iters = (len - 1) / 20;
            do
            {
                uint a = Fetch(s);
                uint b = Fetch(s + 4);
                uint c = Fetch(s + 8);
                uint d = Fetch(s + 12);
                uint e = Fetch(s + 16);
                h += a;
                g += b;
                f += c;
                h = Mur(d, h) + e;
                g = Mur(c, g) + a;
                f = Mur(b + e * c1, f) + d;
                f += g;
                g += f;
                s += 20;
            } while (--iters != 0);
            g = Rotate(g, 11) * c1;
            g = Rotate(g, 17) * c1;
            f = Rotate(f, 11) * c1;
            f = Rotate(f, 17) * c1;
            h = Rotate(h + g, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate(h, 17) * c1;
            h = Rotate(h + f, 19);
            h = h * 5 + 0xe6546b64;
            h = Rotate(h, 17) * c1;
            return h;
        }

        /// <summary>
        /// Calculates a 32bit hash from a given byte array upto a certain length
        /// </summary>
        /// <param name="s">Byte array to calculate the hash on</param>
        /// <param name="len">Number of bytes from the buffer to calculate the hash with</param>
        /// <returns>A 32bit hash</returns>
        public static unsafe uint Hash32(byte[] s, int len)
        {
            // Micro-optimization. Fixing the buffer takes a relatively long
            // time if the buffer is not large and not thoroughly used. For a
            // length of 4 or less, this micro-optimization allowed the
            // algorithm to execute ~15,000 additional iterations per
            // millisecond.
            if (len <= 4)
            {
                return Hash32Len0to4(s, (uint)len);
            }

            fixed (byte* buf = s)
            {
                return Hash32(buf, (uint)len);
            }
        }

        /// <summary>
        /// Calculates a 32bit hash from a given string assuming that the string is
        /// UTF8 encoded. This method is used as a convenience method, if the string
        /// is not UTF8 encoded, call the other hash function with the byte array.
        /// </summary>
        /// <param name="s">String to compute the 64bit hash</param>
        /// <returns>A 32bit hash</returns>
        public static ulong Hash32(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            return Hash32(data, data.Length);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.h#L70
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint128_t Uint128(ulong lo, ulong hi) => new uint128_t(lo, hi);

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L417-L419
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong ShiftMix(ulong val) => val ^ val >> 47;

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L425-L433
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong HashLen16(ulong u, ulong v, ulong mul)
        {
            // Murmur-inspired hashing.
            ulong a = (u ^ v) * mul;
            a ^= a >> 47;
            ulong b = (v ^ a) * mul;
            b ^= b >> 47;
            b *= mul;
            return b;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L472-L483
        // Return a 16-byte hash for 48 bytes.  Quick and dirty.
        // Callers do best to use "random-looking" values for a and b.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint128_t WeakHashLen32WithSeeds(
            ulong w, ulong x, ulong y, ulong z, ulong a, ulong b) {
            a += w;
            b = Rotate64(b + a + z, 21);
            ulong c = a;
            a += x;
            a += y;
            b += Rotate64(a, 44);
            return Uint128(a + z, b + c);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L485-L494
        // Return a 16-byte hash for s[0] ... s[31], a, and b.  Quick and dirty.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint128_t WeakHashLen32WithSeeds(
            byte* s, ulong a, ulong b) {
            return WeakHashLen32WithSeeds(Fetch64(s),
                                        Fetch64(s + 8),
                                        Fetch64(s + 16),
                                        Fetch64(s + 24),
                                        a,
                                        b);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L1710-L1733
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen0to16(byte* s, uint len)
        {
            if (len >= 8) {
                ulong mul = k2 + len * 2;
                ulong a = Fetch64(s) + k2;
                ulong b = Fetch64(s + len - 8);
                ulong c = Rotate64(b, 37) * mul + a;
                ulong d = (Rotate64(a, 25) + b) * mul;
                return HashLen16(c, d, mul);
            }
            if (len >= 4) {
                ulong mul = k2 + len * 2;
                ulong a = Fetch32(s);
                return HashLen16(len + (a << 3), Fetch32(s + len - 4), mul);
            }
            if (len > 0) {
                ushort a = s[0];
                ushort b = s[len >> 1];
                ushort c = s[len - 1];
                uint y = a + ((uint)b << 8);
                uint z = (uint)(len + ((uint)c << 2));
                return ShiftMix(y * k2 ^ z * k0) * k2;
            }
            return k2;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L460-L470
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen17to32(byte* s, uint len)
        {
            ulong mul = k2 + len * 2;
            ulong a = Fetch64(s) * k1;
            ulong b = Fetch64(s + 8);
            ulong c = Fetch64(s + len - 8) * mul;
            ulong d = Fetch64(s + len - 16) * k2;
            return HashLen16(Rotate64(a + b, 43) + Rotate64(c, 30) + d,
                        a + Rotate64(b + k2, 18) + c, mul);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L585-L590
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong H(ulong x, ulong y, ulong mul, int r)
        {
            ulong a = (x ^ y) * mul;
            a ^= a >> 47;
            ulong b = (y ^ a) * mul;
            return Rotate64(b, r) * mul;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L700-L711
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong H32(byte* s, uint len, ulong mul,
                                        ulong seed0 = 0, ulong seed1 = 0) {
            ulong a = Fetch64(s) * k1;
            ulong b = Fetch64(s + 8);
            ulong c = Fetch64(s + len - 8) * mul;
            ulong d = Fetch64(s + len - 16) * k2;
            ulong u = Rotate64(a + b, 43) + Rotate64(c, 30) + d + seed0;
            ulong v = a + Rotate64(b + k2, 18) + c + seed1;
            a = ShiftMix((u ^ v) * mul);
            b = ShiftMix((v ^ a) * mul);
            return b;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L713-L720
        // Return an 8-byte hash for 33 to 64 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen33to64(byte* s, uint len)
        {
            const ulong mul0 = k2 - 30;
            ulong mul1 = k2 - 30 + 2 * len;
            ulong h0 = H32(s, 32, mul0);
            ulong h1 = H32(s + len - 32, 32, mul1);
            return (h1 * mul1 + h0) * mul1;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L722-L730
        // Return an 8-byte hash for 65 to 96 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong HashLen65to96(byte* s, uint len)
        {
            const ulong mul0 = k2 - 114;
            ulong mul1 = k2 - 114 + 2 * len;
            ulong h0 = H32(s, 32, mul0);
            ulong h1 = H32(s + 32, 32, mul1);
            ulong h2 = H32(s + len - 32, 32, mul1, h0, h1);
            return (h2 * 9 + (h0 >> 17) + (h1 >> 21)) * mul1;
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L592-L681
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Hash64_uo(byte* s, uint len)
        {
            const ulong seed0 = 81;
            const ulong seed1 = 0;

            // For strings over 64 bytes we loop.  Internal state consists of
            // 64 bytes: u, v, w, x, y, and z.
            ulong x = seed0;
            ulong y = seed1 * k2 + 113;
            ulong z = ShiftMix(y * k2) * k2;
            uint128_t v = Uint128(seed0, seed1);
            uint128_t w = Uint128(0, 0);
            ulong u = x - z;
            x *= k2;
            ulong mul = k2 + (u & 0x82);

            // Set end so that after the loop we have 1 to 64 bytes left to process.
            byte* end = s + (len - 1) / 64 * 64;
            byte* last64 = end + ((len - 1) & 63) - 63;
            do {
                ulong a0 = Fetch64(s);
                ulong a1 = Fetch64(s + 8);
                ulong a2 = Fetch64(s + 16);
                ulong a3 = Fetch64(s + 24);
                ulong a4 = Fetch64(s + 32);
                ulong a5 = Fetch64(s + 40);
                ulong a6 = Fetch64(s + 48);
                ulong a7 = Fetch64(s + 56);
                x += a0 + a1;
                y += a2;
                z += a3;
                v.first += a4;
                v.second += a5 + a1;
                w.first += a6;
                w.second += a7;

                x = Rotate64(x, 26);
                x *= 9;
                y = Rotate64(y, 29);
                z *= mul;
                v.first = Rotate64(v.first, 33);
                v.second = Rotate64(v.second, 30);
                w.first ^= x;
                w.first *= 9;
                z = Rotate64(z, 32);
                z += w.second;
                w.second += z;
                z *= 9;

                ulong tmp = u;
                u = y;
                y = tmp;

                z += a0 + a6;
                v.first += a2;
                v.second += a3;
                w.first += a4;
                w.second += a5 + a6;
                x += a1;
                y += a7;

                y += v.first;
                v.first += x - y;
                v.second += w.first;
                w.first += v.second;
                w.second += x - y;
                x += w.second;
                w.second = Rotate64(w.second, 34);
                tmp = u;
                u = z;
                z = tmp;
                s += 64;
            } while (s != end);
            // Make s point to the last 64 bytes of input.
            s = last64;
            u *= 9;
            v.second = Rotate64(v.second, 28);
            v.first = Rotate64(v.first, 20);
            w.first += (len - 1) & 63;
            u += y;
            y += u;
            x = Rotate64(y - x + v.first + Fetch64(s + 8), 37) * mul;
            y = Rotate64(y ^ v.second ^ Fetch64(s + 48), 42) * mul;
            x ^= w.second * 9;
            y += v.first + Fetch64(s + 40);
            z = Rotate64(z + w.first, 33) * mul;
            v = WeakHashLen32WithSeeds(s, v.second * mul, x + w.first);
            w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));
            return H(HashLen16(v.first + x, w.first ^ y, mul) + z - u,
                    H(v.second + y, w.second + z, k2, 30) ^ x,
                    k2,
                    31);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L513-L566
        // Return an 8-byte hash for 65 to 96 bytes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Hash64_na(byte* s, uint len)
        {
            const ulong seed = 81;

            // For strings over 64 bytes we loop.  Internal state consists of
            // 56 bytes: v, w, x, y, and z.
            ulong x = seed;
            ulong y = unchecked(seed * k1 + 113);
            ulong z = ShiftMix(y * k2 + 113) * k2;
            uint128_t v = Uint128(0, 0);
            uint128_t w = Uint128(0, 0);
            x = x * k2 + Fetch64(s);

            ulong tmp;

            // Set end so that after the loop we have 1 to 64 bytes left to process.
            byte* end = s + (len - 1) / 64 * 64;
            byte* last64 = end + ((len - 1) & 63) - 63;
            do {
                x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * k1;
                y = Rotate64(y + v.second + Fetch64(s + 48), 42) * k1;
                x ^= w.second;
                y += v.first + Fetch64(s + 40);
                z = Rotate64(z + w.first, 33) * k1;
                v = WeakHashLen32WithSeeds(s, v.second * k1, x + w.first);
                w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

                tmp = z;
                z = x;
                x = tmp;

                s += 64;
            } while (s != end);

            ulong mul = k1 + ((z & 0xff) << 1);
            // Make s point to the last 64 bytes of input.
            s = last64;
            w.first += (len - 1) & 63;
            v.first += w.first;
            w.first += v.first;
            x = Rotate64(x + y + v.first + Fetch64(s + 8), 37) * mul;
            y = Rotate64(y + v.second + Fetch64(s + 48), 42) * mul;
            x ^= w.second * 9;
            y += v.first * 9 + Fetch64(s + 40);
            z = Rotate64(z + w.first, 33) * mul;
            v = WeakHashLen32WithSeeds(s, v.second * mul, x + w.first);
            w = WeakHashLen32WithSeeds(s + 32, z + w.second, y + Fetch64(s + 16));

            tmp = z;
            z = x;
            x = tmp;

            return HashLen16(HashLen16(v.first, w.first, mul) + ShiftMix(y) * k0 + z,
                             HashLen16(v.second, w.second, mul) + x,
                             mul);
        }

        // https://github.com/google/farmhash/blob/34c13ddfab0e35422f4c3979f360635a8c050260/src/farmhash.cc#L732-L748
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong Hash64(byte* s, uint len)
        {
            if (len <= 32) {
                if (len <= 16) {
                    return HashLen0to16(s, len);
                } else {
                    return HashLen17to32(s, len);
                }
            } else if (len <= 64) {
                return HashLen33to64(s, len);
            } else if (len <= 96) {
                return HashLen65to96(s, len);
            } else if (len <= 256) {
                return Hash64_na(s, len);
            } else {
                return Hash64_uo(s, len);
            }
        }

        /// <summary>
        /// Calculates a 64bit hash from a given byte array upto a certain length
        /// </summary>
        /// <param name="s">Byte array to calculate the hash on</param>
        /// <param name="len">Number of bytes from the buffer to calculate the hash with</param>
        /// <returns>A 64bit hash</returns>
        public static unsafe ulong Hash64(byte[] s, int len)
        {
            fixed (byte* buf = s)
            {
                return Hash64(buf, (uint)len);
            }
        }

        /// <summary>
        /// Calculates a 64bit hash from a given string assuming that the string is
        /// UTF8 encoded. This method is used as a convenience method, if the string
        /// is not UTF8 encoded, call the other hash function with the byte array.
        /// </summary>
        /// <param name="s">String to compute the 64bit hash</param>
        /// <returns>A 64bit hash</returns>
        public static ulong Hash64(string s)
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            return Hash64(data, data.Length);
        }
    }
}
