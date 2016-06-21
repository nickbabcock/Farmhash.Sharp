(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/Farmhash.Sharp"

(**
Introducing Farmhash.Sharp
========================

As mentioned on the home page, Farmhash.Sharp is an extremly simple, low-
level, and blazingly fast library for computing [Google's Farmhash][]
algorithm for .NET

[Google's Farmhash]: https://github.com/google/farmhash

If you clone and build this repo, this file `tutorial.fsx` is actually runnable.

Let's start out simple. Let's compute the 32 and 64bit hash of 'hi'.

*)
#r "Farmhash.Sharp.dll"
open Farmhash.Sharp

let text = "hi"

let ``32bit hash`` : uint32 = Farmhash.Hash32(text)
let ``64bit hash`` : uint64 = Farmhash.Hash64(text)

printfn "32bit hash: %x" ``32bit hash``
printfn "64bit hash: %x" ``64bit hash``

(**

The 32bit hash will output a four byte hash (32 bits in length), while the
64bit hash will output an eight byte hash (64 bits in length).

The way Farmhash.Sharp knows how to hash a string of arbitrary encoding is to
look the raw bytes that compose the string. This is great for convenience, but
may not be the best if working directly with arrays or strings across
encodings.

To get bytes from our text, we need to decide on the encoding. Common examples
are ASCII, UTF-8, and Windows-1252. In this tutorial, we're going to keep
things simple and assume that our text is encoded as ASCII

*)

open System.Text
let bytes = Encoding.ASCII.GetBytes(text)

(**

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

For more information on disabling the 32bit preference, see the following [blog
post](http://www.neovolve.com/2015/07/31/disable-prefer-32-bit/).

*)

let ``32bit byte hash`` : uint32 = Farmhash.Hash32(bytes, Array.length bytes)
let ``64bit byte hash`` : uint64 = Farmhash.Hash64(bytes, Array.length bytes)

printfn "32bit byte hash: %x" ``32bit byte hash``
printfn "64bit byte hash: %x" ``64bit byte hash``

(**

Congratulations, you're now an expert at using the Farmhash.Sharp library!

*)
