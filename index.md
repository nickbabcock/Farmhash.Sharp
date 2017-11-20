---
layout: default
---

Farmhash.Sharp is a .NET port of [Google's
Farmhash](https://github.com/google/farmhash) algorithm for calculating 32bit
and 64bit non-cryptographic hashes. Farmhash.Sharp has great performance
characteristics when calculating 64bit hashes, especially on short strings or a
subsequence of byte arrays. See [benchmarks](#comparison-with-other-libraries)
comparing other libraries to Farmhash.Sharp.

## Quickstart

{% highlight csharp %}
using Farmhash.Sharp;

ulong hash = Farmhash.Hash64("Hello world");
{% endhighlight %}

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

## Tutorial

The 32bit hash will output a four byte hash (32 bits in length), while the
64bit hash will output an eight byte hash (64 bits in length).

Let's see the API in action (this time with a byte array).

{% highlight csharp %}
byte[] bytes = new byte[] { /* ... */ };

uint hash32 = Farmhash.Hash32(bytes, bytes.Length)
ulong hash64 = Farmhash.Hash64(bytes, bytes.Length)

Console.WriteLine("32-bit hash: {0}", hash32);
Console.WriteLine("64-bit hash: {0}", hash64);
{% endhighlight %}

Congratulations, you're now an expert at using the Farmhash.Sharp library!

The way Farmhash.Sharp knows how to hash a string of an arbitrary encoding is to
look at the raw bytes that compose the string.

For information on disabling the 32bit preference, see the following [blog
post](http://www.neovolve.com/2015/07/31/disable-prefer-32-bit/).

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

At the time there was not a package on NuGet that satisified these requirements, so I ported farmhash to .NET.

## Benchmarking

Using [BenchmarkDotNet](https://github.com/PerfDotNet/BenchmarkDotNet), the
FarmHash.Sharp benchmarking code pits several non-cryptographic hash functions
against each other in terms of throughput.

Benchmarking was done on the following machine:

```ini
BenchmarkDotNet=v0.10.3.0
OS=Microsoft Windows NT 6.2.9200.0
Processor=Intel(R) Core(TM) i7-6700K CPU @ 4.00GHz, ProcessorCount=8
Frequency=3914059 ticks, Resolution=255.4893 ns
```

To run the benchmarks on the linux machine:

```
sudo ./bench-core-and-mono.sh
```

Please do not take these graphs as absolute truths. Run the benchmark
code yourself to confirm findings.

### Cryptographic vs Non-Cryptographic

For the first graph we will compare a cryptographic (albeit a bad one, as
it's MD5) with Farmhash.Sharp, which is a non-cryptographic hash function.
Whereas a non-cryptographic function only has to optimize against collisions
and speed, a cryptographic function needs to also minimize pathological input.

[![Farmhash-benchmark1](img/crypt-vs-non-crypt.png)](img/crypt-vs-non-crypt.png)

Without getting bogged down into too many specifics, Farmhash easily crushes
MD5.

### 32bit vs 64bit Runtime

What may be surprising is that depending on the runtime Farmhash is running on,
the throughput can be dramatically affected. To show this, I've restricted the
data to only show the 64bit hash of Farmhash across different Clr Jit runtimes to
see which Jit wins.

[![throughput-by-jit](img/throughput-by-jit.png)](img/throughput-by-jit.png)

I suppose the .NET team should be commended, as the latest Jit (their 64bit Ryu Jit)
has 5-10x more throughput than the old Jit with results more pronounced against the
legacy 32bit Jit.

Does mono have the same behavior?

[![mono-throughput](img/mono-throughput.png)](img/mono-throughput.png)

Nope. 32bit and 64bit Mono have approximately the same throughput for 64bit Farmhash.
If you have a keen eye, you may have noticed that the y axis scale changed, which naturally
lends itself to the question of how Mono, Clr, and the new Core runtime compare against
each other.

[![runtime-throughput](img/runtime-throughput.png)](img/runtime-throughput.png)

For both 32bit and 64bit Farmhash functions, the 64bit core and 64bit ryu runtimes
win across any sized payload. Both the core and ryu probably use a lot of the same
code under under the hood.

### Comparison with other libraries

The following benchmark was done:

- For each payload (4, 11, 25, 100, 1000, 10000)
  - For each platform (.NET Core 64bit, Mono 32/64, .NET legacy 32/64, .NET Ryu 64bit)
    - Determine the throughput of calculating a 32bit and 64bit hash

Please click on the image for a better look!

In each configuration, which library has the highest relative throughput compared to competitors in the same row?

[![relative-throughput](img/relative-throughput.png)](img/relative-throughput.png)

Previous heatmap detailed relative throughput, but that was for each facet's
(configuration's) payload size. How can one tell what configuration yields the
highest throughput at a given payload size. Welcome to the next heatmap.

[![absolute-throughput](img/absolute-throughput.png)](img/absolute-throughput.png)

What are some takeaways? Well, if you are constrained to a platform you are
deploying, you'll choose the library that performed the best relative to others
according to your constraints. If you're interested in highest throughput:

- Stick with 64bit hash functions
- Stick with either .NET Core or .NET Ryu
- For small payloads (~ 11 bytes) use Farmhash.Sharp
- For larger payloads, Farmhash.Sharp remains competitive, but [XXHash](https://github.com/ravendb/ravendb/blob/b87a422e91dcf7d4590bad631c9266258be7cab3/Raven.Sparrow/Sparrow/Hashing.cs) found within the Sparrow module of RavenDB is a good option as well.

### C# vs. C++

A good question would be how much efficiency is lost because we're using
C# and not C++, as the original farmhash algorithm uses C++. You can find the
benchmark code [here](https://github.com/nickbabcock/Farmhash.Sharp/tree/5ef3ffc22a1b70b7875dc0b5ae73be496a45fb28/src/Farmhash.Benchmarks).
It uses two versions of the algorithm, one that uses hardware acceleration
([SIMD](https://en.wikipedia.org/wiki/SIMD) instructions), denoted by `-ha`
in the graph, and another compilation that does not use hardware acceleration.

[![Farmhash-benchmark4](img/c-sharp-vs-cpp.png)](img/c-sharp-vs-cpp.png)

I'm pleased to report that for small payloads (<= 25 bytes), Farmhash.Sharp
is around about the fastest if not the fastest. It's only at larger payloads
do we see C++'s lead extend as hardware acceleration becomes more effective.
Still, for large payloads, Farmhash.Sharp has half the throughput as hardware
accelerated C++, which in my opinion, is quite impressive. 

### Conclusion

When deploying on a 64bit application, always choose the 64bit Farmhash
version. If, for whatever reason, Farmhash isn't for you, choose xxHash found
in Ravendb.

Code used to generate the graphs can be found in analysis.R in the github repo.

## Changelog

### 0.4 - November 13th 2017
* Expose internal methods using byte pointers as part of the public interface

### 0.3 - April 25th 2017
* Release Farmhash.Sharp under netstandard 1.0
* Switch to new MSBuild project files

### 0.2 - June 25th 2016
* String helper functions
* 40% faster processing large (> 1KB) inputs

### 0.1 - November 12th 2015
* 32bit Farmhash implemented
* 64bit Farmhash implemented

## Contributing and copyright

The project is hosted on [GitHub][gh] where you can [report issues][issues],
fork the project and submit pull requests. You might also want to read the
[library design notes][readme] for further information.

The library is available under MIT, which allows modification and
redistribution for both commercial and non-commercial purposes. For more information see the
[License file][license] in the GitHub repository.

  [content]: https://github.com/nickbabcock/Farmhash.Sharp/tree/master/docs/content
  [gh]: https://github.com/nickbabcock/Farmhash.Sharp
  [issues]: https://github.com/nickbabcock/Farmhash.Sharp/issues
  [readme]: https://github.com/nickbabcock/Farmhash.Sharp/blob/master/README.md
  [license]: https://github.com/nickbabcock/Farmhash.Sharp/blob/master/LICENSE.txt
