namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("PaperTrail")>]
[<assembly: AssemblyProductAttribute("PaperTrail")>]
[<assembly: AssemblyDescriptionAttribute("A search API for FP papers")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
