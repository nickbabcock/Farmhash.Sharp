module Farmhash.Sharp.Tests

open Farmhash.Sharp
open System.Text
open NUnit.Framework

let (``hash32 len 0 to 4 cases``:obj[][]) = [|
  [| ""; 0xdc56d17au |]
  [| "a"; 0x3c973d4du |]
  [| "ab"; 0x417330fdu |]
  [| "abc"; 0x2f635ec7u |]
  [| "abcd"; 0x98b51e95u |]
  [| "hi"; 0xf2311502u |]
|]

[<Test>]
[<TestCaseSource("hash32 len 0 to 4 cases")>]
let ``hash32 len 0 to 4`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash32(bytes, bytes.Length)
  Assert.AreEqual(expected, actual)


let (``hash32 len 5 to 12 cases``:obj[][]) = [|
  [| "abcde"; 0xa3f366acu |]
  [| "abcdef"; 0x0f813aa4u |]
  [| "abcdefg"; 0x21deb6d7u |]
  [| "abcdefgh"; 0xfd7ec8b9u |]
  [| "abcdefghi"; 0x6f98dc86u |]
  [| "abcdefghij"; 0xf2669361u |]
  [| "hello world"; 0x19a7581au |]
|]

[<Test>]
[<TestCaseSource("hash32 len 5 to 12 cases")>]
let ``hash32 len 5 to 12`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash32(bytes, bytes.Length)
  Assert.AreEqual(expected, actual)

let (``hash32 len 13 to 24 cases``:obj[][]) = [|
  [| "fred@example.com"; 0x7acdc357u |]
  [| "lee@lmmrtech.com"; 0xaf0a30feu |]
  [| "docklandsman@gmail.com"; 0x5d8cdbf4u |]
  [| "size:  a.out:  bad magic"; 0xc6246b8du |]
|]

[<Test>]
[<TestCaseSource("hash32 len 13 to 24 cases")>]
let ``hash32 len 13 to 24`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash32(bytes, bytes.Length)
  Assert.AreEqual(expected, actual)

let (``hash32 larger cases``:obj[][]) = [|
  [| "Go is a tool for managing Go source code.Usage:	go command [arguments]The commands are:    build       compile packages and dependencies    clean       remove object files    env         print Go environment information    fix         run go tool fix on packages    fmt         run gofmt on package sources    generate    generate Go files by processing source    get         download and install packages and dependencies    install     compile and install packages and dependencies    list        list packages    run         compile and run Go program    test        test packages    tool        run specified go tool    version     print Go version    vet         run go tool vet on packagesUse go help [command] for more information about a command.Additional help topics:    c           calling between Go and C    filetype    file types    gopath      GOPATH environment variable    importpath  import path syntax    packages    description of package lists    testflag    description of testing flags    testfunc    description of testing functionsUse go help [topic] for more information about that topic."; 0x9c8f96f3u |]
  [| "Discard medicine more than two years old."; 0xe273108fu |]
  [| "He who has a shady past knows that nice guys finish last."; 0xf585dfc4u |]
  [| "I wouldn't marry him with a ten foot pole."; 0x363394d1u |]
  [| "Free! Free!/A trip/to Mars/for 900/empty jars/Burma Shave"; 0x7613810fu |]
  [| "The days of the digital watch are numbered.  -Tom Stoppard"; 0x2cc30bb7u |]
  [| "Nepal premier won't resign."; 0x322984d9u |]
  [| "For every action there is an equal and opposite government program."; 0xa5812ac8u |]
  [| "His money is twice tainted: 'taint yours and 'taint mine."; 0x1090d244u |]
  [| "There is no reason for any individual to have a computer in their home. -Ken Olsen, 1977"; 0xff16c9e6u |]
  [| "It's a tiny change to the code and not completely disgusting. - Bob Manchek"; 0xcc3d0ff2u |]
  [| "The major problem is with sendmail.  -Mark Horton"; 0xd225e92eu |]
  [| "Give me a rock, paper and scissors and I will move the world.  CCFestoon"; 0x1b8db5d0u |]
  [| "If the enemy is within range, then so are you."; 0x4fda5f07u |]
  [| "It's well we cannot hear the screams/That we create in others' dreams."; 0x2e18e880u |]
  [| "You remind me of a TV show, but that's all right: I watch it anyway."; 0xd07de88fu |]
  [| "C is as portable as Stonehedge!!"; 0x221694e4u |]
  [| "Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley"; 0xe2053c2cu |]
  [| "The fugacity of a constituent in a mixture of gases at a given temperature is proportional to its mole fraction.  Lewis-Randall Rule"; 0x11c493bbu |]
  [| "How can you write a big system without C++?  -Paul Glick"; 0x0819a4e8u |]
|]
[<Test>]
[<TestCaseSource("hash32 larger cases")>]
let ``hash32 larger`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash32(bytes, bytes.Length)
  Assert.AreEqual(expected, actual)

let (``hash64 len 0 to 16 cases``:obj[][]) = [|
  [| ""; 0x9ae16a3b2f90404fUL |]
  [| "a"; 0xb3454265b6df75e3UL |]
  [| "ab"; 0xaa8d6e5242ada51eUL |]
  [| "abc"; 0x24a5b3a074e7f369UL |]
  [| "abcd"; 0x1a5502de4a1f8101UL |]
  [| "abcde"; 0xc22f4663e54e04d4UL |]
  [| "abcdef"; 0xc329379e6a03c2cdUL |]
  [| "abcdefg"; 0x3c40c92b1ccb7355UL |]
  [| "abcdefgh"; 0xfee9d22990c82909UL |]
  [| "abcdefghi"; 0x332c8ed4dae5ba42UL |]
  [| "abcdefghij"; 0x8a3abb6a5f3fb7fbUL |]
  [| "hi"; 0x6a5d2fba44f012f8UL |]
  [| "hello world"; 0x588fb7478bd6b01bUL |]
  [| "lee@lmmrtech.com"; 0x61bec68db00fa2ffUL |]
  [| "fred@example.com"; 0x7fbbcd6191d8dce0UL |]
  [| "size:  a.out:  bad magic"; 0x80d73b843ba57db8UL |]
|]
[<Test>]
[<TestCaseSource("hash64 len 0 to 16 cases")>]
let ``hash64 len 0 to 16`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash64(bytes, uint64 bytes.LongLength)
  Assert.AreEqual(expected, actual)

let (``hash64 len 17 to 32 cases``:obj[][]) = [|
  [| "docklandsman@gmail.com"; 0xb678cf3842309f40UL |]
  [| "Nepal premier won't resign."; 0x8eb3808d1ccfc779UL |]
  [| "C is as portable as Stonehedge!!"; 0xb944f8a16261e414UL |]
|]
[<Test>]
[<TestCaseSource("hash64 len 17 to 32 cases")>]
let ``hash64 len 17 to 32`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash64(bytes, uint64 bytes.LongLength)
  Assert.AreEqual(expected, actual)

let (``hash64 len 33 to 64 cases``:obj[][]) = [|
  [| "Discard medicine more than two years old."; 0x2d072041b535155dUL |]
  [| "He who has a shady past knows that nice guys finish last."; 0x9f9e3cdeb570f926UL |]
  [| "I wouldn't marry him with a ten foot pole."; 0x361b79df08615cd6UL |]
  [| "Free! Free!/A trip/to Mars/for 900/empty jars/Burma Shave"; 0xdcfb73d4de1111c6UL |]
  [| "The days of the digital watch are numbered.  -Tom Stoppard"; 0xd71bdfedb6182a5dUL |]
  [| "His money is twice tainted: 'taint yours and 'taint mine."; 0x3df4b8e109629602UL |]
  [| "The major problem is with sendmail.  -Mark Horton"; 0x1da6c1dfec23a597UL |]
  [| "If the enemy is within range, then so are you."; 0x1f232f3375914f0aUL |]
  [| "How can you write a big system without C++?  -Paul Glick"; 0xa29944470950e8e4UL |]
|]

[<Test>]
[<TestCaseSource("hash64 len 33 to 64 cases")>]
let ``hash64 len 33 to 64`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash64(bytes, uint64 bytes.LongLength)
  Assert.AreEqual(expected, actual)

let (``hash64 larger cases``:obj[][]) = [|
  [| "For every action there is an equal and opposite government program."; 0x8452fbb0c8f98c4fUL |]
  [| "There is no reason for any individual to have a computer in their home. -Ken Olsen, 1977"; 0x7fee06e367562d44UL |]
  [| "It's a tiny change to the code and not completely disgusting. - Bob Manchek"; 0x889b024bab17bf54UL |]
  [| "Give me a rock, paper and scissors and I will move the world.  CCFestoon"; 0xb8e2918a4398348dUL |]
  [| "It's well we cannot hear the screams/That we create in others' dreams."; 0x796229f1faacec7eUL |]
  [| "You remind me of a TV show, but that's all right: I watch it anyway."; 0x98d2fbd5131a5860UL |]
  [| "Even if I could be Shakespeare, I think I should still choose to be Faraday. - A. Huxley"; 0x4c349a4ff7ac0c89UL |]
  [| "The fugacity of a constituent in a mixture of gases at a given temperature is proportional to its mole fraction.  Lewis-Randall Rule"; 0x98eff6958c5e91aUL |]
  [| "Go is a tool for managing Go source code.Usage: go command [arguments]The commands are:    build       compile packages and dependencies    clean       remove object files    env         print Go environment information    fix         run go tool fix on packages    fmt         run gofmt on package sources    generate    generate Go files by processing source    get         download and install packages and dependencies    install     compile and install packages and dependencies    list        list packages    run         compile and run Go program    test        test packages    tool        run specified go tool    version     print Go version    vet         run go tool vet on packagesUse go help [command] for more information about a command.Additional help topics:    c           calling between Go and C    filetype    file types    gopath      GOPATH environment variable    importpath  import path syntax    packages    description of package lists    testflag    description of testing flags    testfunc    description of testing functionsUse go help [topic] for more information about that topic."; 0x21609f6764c635edUL |]
|]
[<Test>]
[<TestCaseSource("hash64 larger cases")>]
let ``hash64 larger`` (str:string) expected =
  let bytes = Encoding.ASCII.GetBytes(str)
  let actual = Farmhash.Hash64(bytes, uint64 bytes.LongLength)
  Assert.AreEqual(expected, actual)