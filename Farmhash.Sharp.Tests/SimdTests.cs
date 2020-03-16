using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Farmhash.Sharp.Tests
{
    public class SimdTests
    {
        [Fact]
        public unsafe void TestLong()
        {
            var sr = new string('a', 512);
            var data = Encoding.ASCII.GetBytes(sr);
            var portable = Farmhash.Hash64(data, data.Length);
            fixed (byte* buf = data)
            {
                var simd = Farmhash.Hash64Simd(buf, (uint)data.Length);
                Assert.Equal(portable, simd);
            }
        }
    }
}
