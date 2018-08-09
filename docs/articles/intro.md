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

`Span<byte>` support is only for `System.Span<T>` compatible backends.

Since 0.4, Farmhash.Sharp contains methods that are part of the public API and
marked `unsafe`. Almost all platforms should be unaffected by this detail.

```csharp
using Farmhash.Sharp;

ulong hash = Farmhash.Hash64("Hello world");

ReadOnlySpan<byte> sp = new byte[] {72, 105};
ulong hash2 = Farmhash.Hash64(sp);
```