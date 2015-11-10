module Farmhash.Sharp.Tests

open Farmhash.Sharp
open NUnit.Framework

[<Test>]
let ``hello returns 42`` () =
  let result = Class1.hello()
  printfn "%i" result
  Assert.AreEqual(42,result)
