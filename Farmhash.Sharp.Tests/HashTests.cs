using System;
using System.Text;
using Xunit;

namespace Farmhash.Sharp.Tests
{
    public class HashTests
    {
        [Theory]
        [InlineData("", 0xdc56d17au)]
        [InlineData("a", 0x3c973d4du)]
        [InlineData("ab", 0x417330fdu)]
        [InlineData("abc", 0x2f635ec7u)]
        [InlineData("abcd", 0x98b51e95u)]
        [InlineData("hi", 0xf2311502u)]
        [InlineData("abcde", 0xa3f366acu)]
        [InlineData("abcdef", 0x0f813aa4u)]
        [InlineData("abcdefg", 0x21deb6d7u)]
        [InlineData("abcdefgh", 0xfd7ec8b9u)]
        [InlineData("abcdefghi", 0x6f98dc86u)]
        [InlineData("abcdefghij", 0xf2669361u)]
        [InlineData("hello world", 0x19a7581au)]
        [InlineData("fred@example.com", 0x7acdc357u)]
        [InlineData("lee@lmmrtech.com", 0xaf0a30feu)]
        [InlineData("docklandsman@gmail.com", 0x5d8cdbf4u)]
        [InlineData("size:  a.out:  bad magic", 0xc6246b8du)]
        [InlineData("Go is a tool for managing Go source code.Usage:	go command [arguments]The commands are:    build       compile packages and dependencies    clean       remove object files    env         print Go environment information    fix         run go tool fix on packages    fmt         run gofmt on package sources    generate    generate Go files by processing source    get         download and install packages and dependencies    install     compile and install packages and dependencies    list        list packages    run         compile and run Go program    test        test packages    tool        run specified go tool    version     print Go version    vet         run go tool vet on packagesUse go help [command] for more information about a command.Additional help topics:    c           calling between Go and C    filetype    file types    gopath      GOPATH environment variable    importpath  import path syntax    packages    description of package lists    testflag    description of testing flags    testfunc    description of testing functionsUse go help [topic] for more information about that topic.", 0x9c8f96f3u)]
        [InlineData("Discard medicine more than two years old.", 0xe273108fu)]
        [InlineData("He who has a shady past knows that nice guys finish last.", 0xf585dfc4u)]
        [InlineData("I wouldn't marry him with a ten foot pole.", 0x363394d1u)]
        [InlineData("Free! Free!/A trip/to Mars/for 900/empty jars/Burma Shave", 0x7613810fu)]
        [InlineData("The days of the digital watch are numbered.  -Tom Stoppard", 0x2cc30bb7u)]
        [InlineData("Nepal premier won't resign.", 0x322984d9u)]
        [InlineData("For every action there is an equal and opposite government program.", 0xa5812ac8u)]
        [InlineData("His money is twice tainted: 'taint yours and 'taint mine.", 0x1090d244u)]
        [InlineData("There is no reason for any individual to have a computer in their home. -Ken Olsen, 1977", 0xff16c9e6u)]
        [InlineData("It's a tiny change to the code and not completely disgusting. - Bob Manchek", 0xcc3d0ff2u)]
        [InlineData("The major problem is with sendmail.  -Mark Horton", 0xd225e92eu)]
        [InlineData("Give me a rock, paper and scissors and I will move the world.  CCFestoon", 0x1b8db5d0u)]
        [InlineData("If the enemy is within range, then so are you.", 0x4fda5f07u)]
        [InlineData("It's well we cannot hear the screams/That we create in others' dreams.", 0x2e18e880u)]
        [InlineData("You remind me of a TV show, but that's all right: I watch it anyway.", 0xd07de88fu)]
        [InlineData("C is as portable as Stonehedge!!", 0x221694e4u)]
        [InlineData("Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley", 0xe2053c2cu)]
        [InlineData("The fugacity of a constituent in a mixture of gases at a given temperature is proportional to its mole fraction.  Lewis-Randall Rule", 0x11c493bbu)]
        [InlineData("How can you write a big system without C++?  -Paul Glick", 0x0819a4e8u)]
        public unsafe void TestHash32(String str, uint expected)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            Assert.Equal(Farmhash.Hash32(bytes, bytes.Length), expected);
            fixed (byte* ptr = bytes)
            {
                Assert.Equal(Farmhash.Hash32(ptr, (uint)bytes.Length), expected);
            }

#if NETCOREAPP2_1
            Span<byte> sp = bytes;
            Assert.Equal(Farmhash.Hash32(sp), expected);
#endif
        }

        [Theory]
        [InlineData("", 0x9ae16a3b2f90404fUL)]
        [InlineData("a", 0xb3454265b6df75e3UL)]
        [InlineData("ab", 0xaa8d6e5242ada51eUL)]
        [InlineData("abc", 0x24a5b3a074e7f369UL)]
        [InlineData("abcd", 0x1a5502de4a1f8101UL)]
        [InlineData("abcde", 0xc22f4663e54e04d4UL)]
        [InlineData("abcdef", 0xc329379e6a03c2cdUL)]
        [InlineData("abcdefg", 0x3c40c92b1ccb7355UL)]
        [InlineData("abcdefgh", 0xfee9d22990c82909UL)]
        [InlineData("abcdefghi", 0x332c8ed4dae5ba42UL)]
        [InlineData("abcdefghij", 0x8a3abb6a5f3fb7fbUL)]
        [InlineData("hi", 0x6a5d2fba44f012f8UL)]
        [InlineData("hello world", 0x588fb7478bd6b01bUL)]
        [InlineData("lee@lmmrtech.com", 0x61bec68db00fa2ffUL)]
        [InlineData("fred@example.com", 0x7fbbcd6191d8dce0UL)]
        [InlineData("size:  a.out:  bad magic", 0x80d73b843ba57db8UL)]
        [InlineData("docklandsman@gmail.com", 0xb678cf3842309f40UL)]
        [InlineData("Nepal premier won't resign.", 0x8eb3808d1ccfc779UL)]
        [InlineData("C is as portable as Stonehedge!!", 0xb944f8a16261e414UL)]
        [InlineData("Discard medicine more than two years old.", 0x2d072041b535155dUL)]
        [InlineData("He who has a shady past knows that nice guys finish last.", 0x9f9e3cdeb570f926UL)]
        [InlineData("I wouldn't marry him with a ten foot pole.", 0x361b79df08615cd6UL)]
        [InlineData("Free! Free!/A trip/to Mars/for 900/empty jars/Burma Shave", 0xdcfb73d4de1111c6UL)]
        [InlineData("The days of the digital watch are numbered.  -Tom Stoppard", 0xd71bdfedb6182a5dUL)]
        [InlineData("His money is twice tainted: 'taint yours and 'taint mine.", 0x3df4b8e109629602UL)]
        [InlineData("The major problem is with sendmail.  -Mark Horton", 0x1da6c1dfec23a597UL)]
        [InlineData("If the enemy is within range, then so are you.", 0x1f232f3375914f0aUL)]
        [InlineData("How can you write a big system without C++?  -Paul Glick", 0xa29944470950e8e4UL)]
        [InlineData("For every action there is an equal and opposite government program.", 0x8452fbb0c8f98c4fUL)]
        [InlineData("There is no reason for any individual to have a computer in their home. -Ken Olsen, 1977", 0x7fee06e367562d44UL)]
        [InlineData("It's a tiny change to the code and not completely disgusting. - Bob Manchek", 0x889b024bab17bf54UL)]
        [InlineData("Give me a rock, paper and scissors and I will move the world.  CCFestoon", 0xb8e2918a4398348dUL)]
        [InlineData("It's well we cannot hear the screams/That we create in others' dreams.", 0x796229f1faacec7eUL)]
        [InlineData("You remind me of a TV show, but that's all right: I watch it anyway.", 0x98d2fbd5131a5860UL)]
        [InlineData("Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley", 0x4c349a4ff7ac0c89UL)]
        [InlineData("The fugacity of a constituent in a mixture of gases at a given temperature is proportional to its mole fraction.  Lewis-Randall Rule", 0x98eff6958c5e91aUL)]
        [InlineData("Go is a tool for managing Go source code.Usage: go command [arguments]The commands are:    build       compile packages and dependencies    clean       remove object files    env         print Go environment information    fix         run go tool fix on packages    fmt         run gofmt on package sources    generate    generate Go files by processing source    get         download and install packages and dependencies    install     compile and install packages and dependencies    list        list packages    run         compile and run Go program    test        test packages    tool        run specified go tool    version     print Go version    vet         run go tool vet on packagesUse go help [command] for more information about a command.Additional help topics:    c           calling between Go and C    filetype    file types    gopath      GOPATH environment variable    importpath  import path syntax    packages    description of package lists    testflag    description of testing flags    testfunc    description of testing functionsUse go help [topic] for more information about that topic.", 0x21609f6764c635edUL)]
        public unsafe void TestHash64(String str, ulong expected)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            Assert.Equal(Farmhash.Hash64(bytes, bytes.Length), expected);
            fixed (byte* ptr = bytes)
            {
                Assert.Equal(Farmhash.Hash64(ptr, (uint)bytes.Length), expected);
            }

#if NETCOREAPP2_1
            Span<byte> sp = bytes;
            Assert.Equal(Farmhash.Hash64(sp), expected);
#endif
        }

#if NETCOREAPP2_1
        [Fact]
        public void TestHash64Span()
        {
            var data = new byte[] {1};
            Span<byte> sp = data;
            var expected = Farmhash.Hash64(data, data.Length);
            Assert.Equal(Farmhash.Hash64(sp), expected);
        }

        [Fact]
        public void TestHash32Span()
        {
            var data = new byte[] {1};
            Span<byte> sp = data;
            var expected = Farmhash.Hash32(data, data.Length);
            Assert.Equal(Farmhash.Hash32(sp), expected);
        }
#endif
    }
}
