# Farmhash.Sharp

[Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp) is a .NET port
of [Google's Farmhash](https://github.com/google/farmhash) algorithm for
calculating 32bit and 64bit non-cryptographic hashes. Farmhash.Sharp has great
performance characteristics when calculating 64bit hashes, especially on short
strings or a subsequence of byte arrays.

## Quickstart

```
dotnet add package Farmhash.Sharp
```

```csharp
using Farmhash.Sharp;

ulong hash64 = Farmhash.Hash64("Hello world");
uint hash32 = Farmhash.Hash32("Hello world");

var data = new byte[] {72, 105};
ulong dhash64 = Farmhash.Hash64(data, data.Length);

// For .NET core app 2.1+
ReadOnlySpan<byte> sp = data;
ulong hash2 = Farmhash.Hash64(sp);
```

## Links

- [Overview](articles/intro.md)
- [Motivation behind this library](articles/motivation.md)
- [Compare C# farmhash performance to C++](articles/benchmarks.md#c-vs-c)
- [Compare C# farmhash to other C# hash libraries](articles/benchmarks.md#comparison-with-other-libraries)