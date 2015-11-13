# Farmhash.Sharp

To build everything:

    > build.cmd // on windows    
    $ ./build.sh  // on unix
    
Documentation: http://nickbabcock.github.io/Farmhash.Sharp

## Motiviation

On a regular basis, I need a non-cryptographic hash calculated from a portion of a byte array. This byte array
may end of becoming a string, but allocating a string just to throw it away for `GetHashCode` caused too much
GC pressure and resulted in degraded performance. Additionally, `GetHashCode` produces only a 32bit hash

My requirements became:

* Must be able to operate on a subsequence of a byte array
* Must be able to produce a 64bit output
* Must produce a hash with a low chance of collision
* Must make zero heap allocations
* Must be fast for short subsequences (many of the strings I see are short)

When dealing with 64bit hashes there is a 1 in a billion chance for collision given two hundred thousand
values, which aligns with my domain perfectly. Whereas, for 32bit hashes, two hundred thousand values is
almost guaranteed to produce a collision. For more information on collision probabilities, see the [Preshing
on Programming article][]

[Preshing on Programming article]: http://preshing.com/20110504/hash-collision-probabilities/

## Build Status

Mono | .NET
---- | ----
[![Mono CI Build Status](https://img.shields.io/travis/nickbabcock/Farmhash.Sharp/master.svg)](https://travis-ci.org/nickbabcock/Farmhash.Sharp) | [![Build status](https://ci.appveyor.com/api/projects/status/w550d3vtweb4vsmq?svg=true)](https://ci.appveyor.com/project/nickbabcock/farmhash-sharp)


