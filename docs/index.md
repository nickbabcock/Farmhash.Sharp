# Farmhash.Sharp

[Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp) is a .NET port
of [Google's Farmhash](https://github.com/google/farmhash) algorithm for
calculating 32bit and 64bit non-cryptographic hashes. Farmhash.Sharp has great
performance characteristics when calculating 64bit hashes, especially on short
strings or a subsequence of byte arrays.

## Quickstart

```csharp
using Farmhash.Sharp;

ulong hash64 = Farmhash.Hash64("Hello world");
uint hash32 = Farmhash.Hash32("Hello world");
```

## Links

- [Installation](/articles/intro.html)
- [Motivation behind this library](/articles/motivation.html)
- [Compare C# farmhash performance to C++](/articles/benchmarks.html#c-vs-c)
- [Compare C# farmhash to other C# hash libraries](/articles/benchmarks.html#comparison-with-other-libraries)