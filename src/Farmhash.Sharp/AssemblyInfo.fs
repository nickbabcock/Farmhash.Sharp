namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("Farmhash.Sharp")>]
[<assembly: AssemblyProductAttribute("Farmhash.Sharp")>]
[<assembly: AssemblyDescriptionAttribute("Port of Google's farm hash algorithm")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
