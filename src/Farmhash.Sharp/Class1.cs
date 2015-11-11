// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_BuiltInTypes
namespace Farmhash.Sharp
{
    public class Class1
    {
        // Some primes between 2^63 and 2^64 for various uses.
        const ulong k0 = 0xc3a5c85c97cb3127U;
        const ulong k1 = 0xb492b66fbe98f273U;
        const ulong k2 = 0x9ae16a3b2f90404fU;

        // Magic numbers for 32-bit hashing.  Copied from Murmur3.
        const uint c1 = 0xcc9e2d51;
        const uint c2 = 0x1b873593;

        private static uint Rotate32(uint val, int shift)
        {
            return shift == 0 ? val : (val >> shift) | (val << (32 - shift));
        }

        private ulong Rotate64(ulong val, int shift)
        {
            return shift == 0 ? val : (val >> shift) | (val << (64 - shift));
        }

        public static uint Rotate(uint val, int shift)
        {
            return Rotate32(val, shift);
        }

        private static unsafe ulong Fetch64(byte* p)
        {
            return *(ulong*) p;
        }

        private static unsafe uint Fetch32(byte* p)
        {
            return *(uint*)p;
        }

        private static unsafe uint Fetch(byte* p)
        {
            return Fetch32(p);
        }

        private static uint fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        private static unsafe uint Hash32Len0to4(byte* s, int len)
        {
            uint b = 0;
            uint c = 9;
            for (int i = 0; i < len; i++)
            {
                sbyte v = (sbyte) s[i];
                b = (uint) (b * c1 + v);
                c ^= b;
            }
            return fmix(Mur(b, Mur((uint) len, c)));
        }

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

        private static unsafe uint Hash32Len13to24(byte* s, int len, uint seed = 0) {
            uint a = Fetch(s - 4 + (len >> 1));
            uint b = Fetch(s + 4);
            uint c = Fetch(s + len - 8);
            uint d = Fetch(s + (len >> 1));
            uint e = Fetch(s);
            uint f = Fetch(s + len - 4);
            uint h = (uint) (d * c1 + len + seed);
            a = Rotate(a, 12) + f;
            h = Mur(c, h) + a;
            a = Rotate(a, 3) + c;
            h = Mur(e, h) + a;
            a = Rotate(a + f, 12) + d;
            h = Mur(b ^ seed, h) + a;
            return fmix(h);
        }

        private static unsafe uint Hash32Len5to12(byte* s, int len, uint seed = 0)
        {
            uint a = (uint) len, b = (uint) (len * 5), c = 9, d = b + seed;
            a += Fetch(s);
            b += Fetch(s + len - 4);
            c += Fetch(s + ((len >> 1) & 4));
            return fmix(seed ^ Mur(c, Mur(b, Mur(a, d))));
        }

        private static unsafe uint Hash32(byte* s, int len)
        {
            if (len <= 24)
            {
                return len <= 12 ?
                    (len <= 4 ? Hash32Len0to4(s, len) : Hash32Len5to12(s, len)) :
                    Hash32Len13to24(s, len);
            }

            uint h = (uint) len, g = (uint) (c1 * len), f = g;
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
            int iters = (len - 1) / 20;
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


        public static unsafe uint Hash32(byte[] s, int len)
        {
            fixed (byte* buf = s)
            {
                return Hash32(buf, len);
            }
        }
    }
}
