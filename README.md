[![Build Status](https://dev.azure.com/nbabcock19/nbabcock19/_apis/build/status/nickbabcock.Farmhash.Sharp?branchName=master)](https://dev.azure.com/nbabcock19/nbabcock19/_build/latest?definitionId=5&branchName=master)

# Farmhash.Sharp

[Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp) is a .NET port
of [Google's Farmhash](https://github.com/google/farmhash) algorithm for
calculating 32bit and 64bit non-cryptographic hashes. Farmhash.Sharp has great
performance characteristics when calculating 64bit hashes, especially on short
strings or a subsequence of byte arrays.

[**Main Documentation**](https://nickbabcock.github.io/Farmhash.Sharp/index.html)

Links:

- [Overview](https://nickbabcock.github.io/Farmhash.Sharp/articles/intro.html)
- [NuGet Link](https://www.nuget.org/packages/Farmhash.Sharp/)
- [Motivation behind this library](https://nickbabcock.github.io/Farmhash.Sharp/articles/motivation.html)
- [Compare C# farmhash performance to C++](https://nickbabcock.github.io/Farmhash.Sharp/articles/benchmarks.html#c-vs-c)
- [Compare C# farmhash to other C# hash libraries](https://nickbabcock.github.io/Farmhash.Sharp/articles/benchmarks.html#comparison-with-other-libraries)


## Building

To build and test everything with the .NET Core 2.1 SDK:

```
dotnet test Farmhash.Test/Farmhash.Test.csproj
```
