[![Build Status](https://travis-ci.org/nickbabcock/Farmhash.Sharp.svg?branch=master)](https://travis-ci.org/nickbabcock/Farmhash.Sharp) [![Build status](https://ci.appveyor.com/api/projects/status/w550d3vtweb4vsmq?svg=true)](https://ci.appveyor.com/project/nickbabcock/farmhash-sharp) [![codecov](https://codecov.io/gh/nickbabcock/Farmhash.Sharp/branch/master/graph/badge.svg)](https://codecov.io/gh/nickbabcock/Farmhash.Sharp)

# Farmhash.Sharp

[Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp) is a .NET port
of [Google's Farmhash](https://github.com/google/farmhash) algorithm for
calculating 32bit and 64bit non-cryptographic hashes. Farmhash.Sharp has great
performance characteristics when calculating 64bit hashes, especially on short
strings or a subsequence of byte arrays.

[**Main Documentation**](https://nickbabcock.github.io/Farmhash.Sharp/index.html)

Links:

- [Overview](https://nickbabcock.github.io/Farmhash.Sharp/articles/intro.html)
- [Motivation behind this library](https://nickbabcock.github.io/Farmhash.Sharp/articles/motivation.html)
- [Compare C# farmhash performance to C++](https://nickbabcock.github.io/Farmhash.Sharp/articles/benchmarks.html#c-vs-c)
- [Compare C# farmhash to other C# hash libraries](https://nickbabcock.github.io/Farmhash.Sharp/articles/benchmarks.html#comparison-with-other-libraries)


## Building

To build and test everything with the .NET Core 2.1 SDK:

```
dotnet test -f netcoreapp2.1 Farmhash.Test/Farmhash.Test.csproj
```
