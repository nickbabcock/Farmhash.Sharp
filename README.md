Travis: [![Build Status](https://travis-ci.org/nickbabcock/Farmhash.Sharp.svg?branch=master)](https://travis-ci.org/nickbabcock/Farmhash.Sharp) Appveyor: [![Build status](https://ci.appveyor.com/api/projects/status/w550d3vtweb4vsmq?svg=true)](https://ci.appveyor.com/project/nickbabcock/farmhash-sharp)


# Farmhash.Sharp

Farmhash.Sharp is an extremely simple, low-level, and blazingly fast library
for computing [Google's Farmhash][] algorithm for .NET. This repo contains a host
of benchmark across various runtimes and hashing algorithms and none can compare
to the all around performance of Farmhash.Sharp.

Install from [NuGet](https://www.nuget.org/packages/Farmhash.Sharp/):

```
dotnet add package Farmhash.Sharp
```

Farmhash.Sharp is built against NET Standard 1.0 so can be ran on any of the following:

- Full .NET Framework
- Mono
- .NET Core

[Documentation](https://nickbabcock.github.io/Farmhash.Sharp)

[Google's Farmhash]: https://github.com/google/farmhash

To build everything, you'll need a recent version of the .NET Core SDK:

```
dotnet restore
dotnet build
dotnet test Farmhash.Test/Farmhash.Test.csproj
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
