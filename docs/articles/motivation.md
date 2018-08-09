# Motivation

On a regular basis, I need a non-cryptographic hash calculated from a portion of a byte array. This byte array
may end up becoming a string, but allocating a string just to throw it away for `GetHashCode` caused too much
GC pressure and resulted in degraded performance. Additionally, `GetHashCode` produces only a 32bit hash

My requirements became:

* Must be able to operate on a leading subsequence of a byte array
* Must be able to produce a 64bit output
* Must produce a hash with a low chance of collision
* Must make zero heap allocations
* Must be fast for short subsequences (many of the strings I see are short)

At the time there was not a package on NuGet that satisified these requirements, so I ported farmhash to .NET.