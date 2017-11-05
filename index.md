---
layout: default
---

Farmhash.Sharp is an extremly simple, low-level, non-cryptographic, and
blazingly fast library for computing [Google's Farmhash][] algorithm for .NET.
[Install from NuGet](http://www.nuget.org/packages/Farmhash.Sharp/)

[Google's Farmhash]: https://github.com/google/farmhash

## Quick Sample

{% highlight C# %}

using Farmhash.Sharp;
using System.Text;

// Let's print out the 32 and 64 bit Farmhash
// for ascii encoded "hello"
string str = "hello";
byte[] data = Encoding.ASCII.GetBytes(str);
uint hash32 = Farmhash.Hash32(data, data.Length);
Console.WriteLine("32-bit hash: {0}", hash32);
// - will print 2039911270

ulong hash64 = Farmhash.Hash64(data, data.Length);
Console.WriteLine("64-bit hash: {0}", hash64);
// - will print 13009744463427800296

// And for the lazy, one can elect to not choose an encoding
Console.WriteLine("32-bit hash: {0}", Farmhash.Hash32(str));
// - in this case will print 1185801527 and may 
//   not be the same in future versions
{% endhighlight %}

## Tutorial

The 32bit hash will output a four byte hash (32 bits in length), while the
64bit hash will output an eight byte hash (64 bits in length).

The way Farmhash.Sharp knows how to hash a string of arbitrary encoding is to
look at the raw bytes that compose the string. This is great for convenience,
but may not be the best if working directly with arrays or strings across
encodings.

To get bytes from our text, we need to decide on the encoding. Common examples
are ASCII, UTF-8, and Windows-1252. In this tutorial, we're going to keep
things simple and assume that our text is encoded as ASCII

{% highlight C# %}

using System.Text;
byte[] bytes = Encoding.ASCII.GetBytes(text)

{% endhighlight %}

With our bytes handy, it is now time to calculate the hash! Choosing which
hash to use may be the hardest decision in this library. If you need the
lowest probability of collisions, then your choice is simple, go with Hash64.
If you need the fastest speed then it depends on the architecture of the
machine being ran on and how your project is compiled:

- A project executed on a 32bit machine should always prefer Hash32.
- A project executed on a 64bit machine, but prefers 32bit, should use Hash32
- A project executed on a 64bit machine, but doesn't prefer 32bit, should
  always use Hash64, even if only a 32bit hash is wanted. The 64bit hash is
  always faster.

See the benchmarking section for concrete numbers.

For more information on disabling the 32bit preference, see the following [blog
post](http://www.neovolve.com/2015/07/31/disable-prefer-32-bit/).

And for good measure, let's see the API in action once again.

{% highlight C# %}

uint hash32 = Farmhash.Hash32(bytes, bytes.Length)
ulong hash64 = Farmhash.Hash64(bytes, bytes.Length)

Console.WriteLine("32-bit hash: {0}", hash32);
Console.WriteLine("64-bit hash: {0}", hash64);

{% endhighlight %}

Congratulations, you're now an expert at using the Farmhash.Sharp library!

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

In each configuration, which library has the highest throughput compared competitors?

[![relative-throughput](img/relative-throughput.png)](img/relative-throughput.png)

Previous heatmap detailed relative throughput, but that was for each facet's
payload size. How can one tell if in terms of absolute throughput what
configuration yields the highest throughput at a given payload size. Welcome to
the next heatmap.

[![absolute-throughput](img/absolute-throughput.png)](img/absolute-throughput.png)

What are some takeaways? Well, if you are constrained to a platform you are
deploying, you'll choose the library that performed the best relative to others
according to your constraints. If you're interested in highest throughput:

- Stick with 64bit hash functions
- Stick with either .NET Core or .NET Ryu
- For small payloads (~ 11 bytes) use Farmhash.Sharp
- For larger payloads, Farmhash.Sharp remains competitive, but the XXHash found in Sparrow's codebase and Spookily are good options as well.

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
