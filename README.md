Travis: [![Build Status](https://travis-ci.org/nickbabcock/Farmhash.Sharp.svg?branch=master)](https://travis-ci.org/nickbabcock/Farmhash.Sharp) Appveyor: [![Build status](https://ci.appveyor.com/api/projects/status/w550d3vtweb4vsmq?svg=true)](https://ci.appveyor.com/project/nickbabcock/farmhash-sharp)

# Farmhash.Sharp

[Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp) is a .NET port
of [Google's Farmhash](https://github.com/google/farmhash) algorithm for
calculating 32bit and 64bit non-cryptographic hashes. Farmhash.Sharp has great
performance characteristics when calculating 64bit hashes, especially on short
strings or a subsequence of byte arrays. See
[benchmarks](https://nickbabcock.github.io/Farmhash.Sharp#comparison-with-other-libraries)
comparing other libraries to Farmhash.Sharp.

## Quickstart

```csharp
using Farmhash.Sharp;

ulong hash = Farmhash.Hash64("Hello world");
```

## Installation and Compatibility

Install from [NuGet](https://www.nuget.org/packages/Farmhash.Sharp/):

```
dotnet add package Farmhash.Sharp
```

Farmhash.Sharp is built against NET Standard 1.0 so can be ran on any of the following:

- Full .NET Framework
- Mono
- .NET Core

Since 0.4, Farmhash.Sharp contains methods that are part of the public API and
marked `unsafe`. Almost all platforms should be unaffected by this detail.

## Building

To build and test everything with the .NET Core 2.0 SDK:

```
dotnet test -f netcoreapp2.0 Farmhash.Test/Farmhash.Test.csproj
```

## Motivation

On a regular basis, I need a non-cryptographic hash calculated from a portion of a byte array. This byte array
may end up becoming a string, but allocating a string just to throw it away for `GetHashCode` caused too much
GC pressure and resulted in degraded performance. Additionally, `GetHashCode` produces only a 32bit hash

My requirements became:

* Must be able to operate on a leading subsequence of a byte array
* Must be able to produce a 64bit output
* Must produce a hash with a low chance of collision
* Must make zero heap allocations
* Must be fast for short subsequences (many of the strings I see are short)

When dealing with 64bit hashes there is a 1 in a billion chance for collision given two hundred thousand
values, which aligns with my domain perfectly. Whereas, for 32bit hashes, two hundred thousand values is
almost guaranteed to produce a collision. For more information on collision probabilities, see the [Preshing
on Programming article][]

[Preshing on Programming article]: http://preshing.com/20110504/hash-collision-probabilities/
