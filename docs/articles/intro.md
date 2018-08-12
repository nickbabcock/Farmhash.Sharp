# Overview

## Installation

Install from [NuGet](https://www.nuget.org/packages/Farmhash.Sharp/):

```
dotnet add package Farmhash.Sharp
```

Farmhash.Sharp is built against NET Standard 1.0 so can be ran on any of the following:

- Full .NET Framework
- Mono
- .NET Core

## Span Support

[Span support](https://msdn.microsoft.com/en-us/magazine/mt814808.aspx) is only for backends which don't require additional dependencies. This limits span usage in Farmhash.Sharp to only those apps targeting a .NET core 2.1 environment. In the future, there may be a way for users interested in span support for .net standard to opt into it.

## Walkthrough

The surface area of Farmhash.Sharp is very small, so small in fact that every method can be listed without being overwhelming:

```csharp
uint Hash32(byte *s, int length);
uint Hash32(byte[] s, int length);
uint Hash32(ReadOnlySpan<byte> span);
uint Hash32(string s);

ulong Hash64(byte *s, int length);
ulong Hash64(byte[] s, int length);
ulong Hash64(ReadOnlySpan<byte> span);
ulong Hash64(string s);
```

Examples on how to use the APIs:

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

The recommendation is to use 64bit hash in almost all circumstances because more resistant to hash collisions and it is [faster](/articles/benchmarks.html).