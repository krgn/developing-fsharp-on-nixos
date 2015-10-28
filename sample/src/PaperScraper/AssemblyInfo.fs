namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("PaperScraper")>]
[<assembly: AssemblyProductAttribute("PaperScraper")>]
[<assembly: AssemblyDescriptionAttribute("Project has no summmary; update build.fsx")>]
[<assembly: AssemblyVersionAttribute("1.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0"
