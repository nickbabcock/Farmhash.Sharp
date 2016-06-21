(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use
// it to define helpers that you do not want to show in the documentation.
#I "../../bin/Farmhash.Sharp"

(**
Farmhash.Sharp
======================

Farmhash.Sharp is an extremly simple, low-level, and blazingly fast library
for computing [Google's Farmhash][] algorithm for .NET

<div class="row">
  <div class="span1"></div>
  <div class="span6">
    <div class="well well-small" id="nuget">
      The Farmhash.Sharp library can be <a href="https://nuget.org/packages/Farmhash.Sharp">installed from NuGet</a>:
      <pre>PM> Install-Package Farmhash.Sharp</pre>
    </div>
  </div>
  <div class="span1"></div>
</div>

Example
-------

Below are a couple of examples in F# (but Farmhash.Sharp is equally usable from
C#, VB.NET, and any .NET language).

[Google's Farmhash]: https://github.com/google/farmhash

*)
#r "Farmhash.Sharp.dll"
open Farmhash.Sharp

printfn "'hello' 32bit hash: %x" (Farmhash.Hash32("hello"))
printfn "'hello' 64bit hash: %x" (Farmhash.Hash64("hello"))

let arr = [| 1uy .. 100uy |]
printfn "byte array 64bit hash: %x" (Farmhash.Hash64(arr, arr.Length))

(**
Some additional resources

 * [Tutorial](tutorial.html) contains a walkthrough of the library.

 * [Benchmarks](benchmarks.html) details how this port of Farmhash stacks up against other hashing algorithms

 * [API Reference](reference/index.html) contains automatically generated documentation for all types, modules
   and functions in the library. This includes additional brief samples on using most of the
   functions.

Contributing and copyright
--------------------------

The project is hosted on [GitHub][gh] where you can [report issues][issues], fork
the project and submit pull requests. If you're adding a new public API, please also
consider adding [samples][content] that can be turned into a documentation. You might
also want to read the [library design notes][readme] to understand how it works.

The library is available under MIT, which allows modification and
redistribution for both commercial and non-commercial purposes. For more information see the
[License file][license] in the GitHub repository.

  [content]: https://github.com/nickbabcock/Farmhash.Sharp/tree/master/docs/content
  [gh]: https://github.com/nickbabcock/Farmhash.Sharp
  [issues]: https://github.com/nickbabcock/Farmhash.Sharp/issues
  [readme]: https://github.com/nickbabcock/Farmhash.Sharp/blob/master/README.md
  [license]: https://github.com/nickbabcock/Farmhash.Sharp/blob/master/LICENSE.txt
*)
