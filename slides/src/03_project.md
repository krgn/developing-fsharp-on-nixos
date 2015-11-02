# A Sample Project

#### Project

*****

#### Proposition

Assume we have a lots of great computer science papers on our SSD, and we'd like to be
able to index and query for information (e.g. [1]) via _curl_.

So, lets build a small microservice around the _recoll_ full-text indexer and
serve query results via HTTP.

[1] https://github.com/ocharles/papers

<div class="notes">
- idea could at transferred to some more relevant technology like, e.g., _elasticsearch_ 
- from there, it might be interesting to go on to explore clustering and config management with nix
</div>

*****

#### A quick word about...

*****

#### Microservices!

*****

#### Microservies?

_TODO_: remind myself WTF are they again?

*****

No, but seriously, its a stupid buzzword and we all know it but apparently we
like pressing those buttons over and over again.

*****

A microservice is a small, stand-alone component most often part of a system of
more of these stand-alone, de-coupled units.

Essentially, its a scalability design pattern for web applications.

*****

From the point of view of a functional programming enthusiast, microservices are
a good example of how small, stateless (_pure!_) building blocks (_referential
transparency_) can be _composed_ into systems that are easier to understand and
maintain and more robust that big monolithic code-bases.

*****

I am not a dev-ops person, but I hear that larger systems become fiendishly hard
to deploy and monitor, though.

Blame successfully shifted!

:)

<div class="notes">
</div>

## Bootstrapping

*****

The state of affairs of project management in mono/F# is still for the most part
centered around using IDE's for everything.

<div class="notes">
- quite a big annoyance
</div>

*****

There are the obvious Candidates, such as MS' Visual Studio or MonoDevelop, which
is at least free and available via _nix_.

*****

To alleviate that situation for those who don't like IDEs, there is a project
scaffold git repository with an initialization routine to help set up
everything.

*****

We'll focus on that workflow for this talk, since this be beneficial to automate
tasks down the line (think CI and deployment).

*****

#### Using Project Scaffold

```{.fsharp}
→ git clone git@github.com:fsprojects/ProjectScaffold.git PaperScraper
```

```{.fsharp .fragment}
→ cd PaperScraper && ./build.sh
```

*****

Answer a couple of questions and you're set. But wait!

*****

![](img/splonk.jpg)

*****

```error : Target named 'Rebuild' not found in the project.```

*****

#### A Note About Build Targets

Since in _NixOS_ there is no(t one, but many) Global Assembly Cache, resolving
default build targets shipped with _F#_ does not work as expected.

*****

The solution is to patch all _.fsproj_ files in the solution and only
conditionally set the _FSharpTargetsPath_ if the target actually exists:

```
- <FSharpTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio...
+ <FSharpTargetsPath Condition="Exists('$(MSBuildExtensionsPath32)\M...
```

*****

There is a package _nix_ written by _@obadz_ which contains a script that finds
and patches up _.fsproj_ files to look out for the _FSharpTargetsPath_
enviroment variable.

```→ nix-env -i dotnetbuildhelpers```

*****

#### Usage: 

```
→ patch-fsharp-targets.sh
  Patching F# targets in fsproj files...
  ./src/PaperScraper/PaperScraper.fsproj
  ./tests/PaperScraper.Tests/PaperScraper.Tests.fsproj
```

*****

Last, we only need to set _FSharpTargetsPath_ in our shell:

```{.shell}
→ export FSharpTargetsPath=$(dirname $(which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets
```

```{.shell .fragment}
→ set -x FSharpTargetsPath (dirname (which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets
```

```{.shell .fragment}
{ config, pkgs, ... }:
{
  environment.variables.FSharpTargetsPath = "${pkgs.fsharp}/lib/mono/4.5/Microsoft.FSharp.Targets";
}
```

*****

#### Hm, More Errors

Unfortuantely, more problems crop up at this point.

- FSharp.Core dll is missing when running tests, hence the build fails
- FSharp.Core dll is missing when generating documentation, hence the build fails

:(

<div class="notes">
- mono crashes really hard running the tests
</div>

*****

#### Ah yes!

> "In compiled applications, you should never assume that FSharp.Core is in the
> GAC ("Global Assembly Cache"). Instead, you should deploy the appropriate
> FSharp.Core as part of your application."

https://fsharp.github.io/2015/04/18/fsharp-core-notes.html

*****

#### Quick Fix:

Ship _FSharp.Core_ as part of the build output and manage the dependency with _paket_.

```{.fragment}
in paket.dependencies, add;

FSharp.Core = 4.0.0.1
```
```{.fragment}
in src/PaperScraper/paket.references, add:

FSharp.Core
```

```{.fragment}
<Reference Include="FSharp.Core">
  <Private>True</Private>
  <HintPath>..\..\packages\FSharp.Core\lib\net40\FSharp.Core.dll</HintPath>
</Reference>
```

<div class="notes">
- add the FSharp.Core and verision to paket.dependencies
- add the FSharp.Core to paket.references in the project and tests project
- make sure to set TargetFSharpCoreVersion to the correct version!
- execute paket install again
- mono crashes really hard running the tests
</div>

*****

#### A Better™ Fix:

It would be better to use the F# version shipped with NixOS.

```{.fragment}
  <TargetFSharpCoreVersion>4.3.1.0</TargetFSharpCoreVersion>
```

```{.fragment}
  <HintPath>$(TargetFSharpCorePath)</HintPath>
```

```{.shell .fragment}
→ set -x TargetFSharpCorePath (dirname (which fsharpc))/../lib/mono/4.5/FSharp.Core.dll
```

```{.shell .fragment}
{ config, pkgs, ... }:
{
  environment.variables.TargetFSharpCorePath = "${pkgs.fsharp}/lib/mono/4.5/FSharp.Core.dll";
}
```
<div class="notes">
- this is more general and robust 
</div>

*****

#### Finding the correct F\# Version 

![](img/fsharp-versions.png)

http://stackoverflow.com/questions/20332046/correct-version-of-fsharp-core

*****

#### Documentation and Help Targets

They do not build at this point, for the same reasons.

TODO: improve this slide

*****

#### But we're building an executable, right?!

The project template at this point generates a library project by default, so
the _.fsproj_ file needs to be amended in 2 ways:

```{.fragment}
    <OutputType>Exe</OutputType>
```

```{.fragment}
    <ErrorReport>prompt</ErrorReport>
    <Externalconsole>true</Externalconsole>
```

*****

Additionally, the tests project also holds a reference to the current project,
so we need to comment it and the code in _Tests.fs_ out to ensure a clean build.

<div class="notes">
</div>

*****

## Phew!


*****

## Recoll

> - full-text search tool 
> - uses xapian underneath (like other great tools, e.g. `notmuch` and `mu`)
> - extensible and configurable
> - supports indexing many mime types, including extracting text from PDFs 
> - has a flexible query language (based on Xesam)
> - needs to be configured correctly (whitelist directories)
> - the indexer needs to be run in a cron/systemd timer job

*****

#### An Example

```
➜  recoll -t -n 1 -m -q burrito
Recoll query: ((burrito:(wqf=11) OR burritos))
5 results (printing  1 max):
application/pdf	[file:///home/k/doc/books/burrito_monads.pdf]	[burrito_monads.pdf]	74745	bytes	
abstract =   Burritos for the Hungry Mathematician Ed Morehouse April 1, 2015 Abstract The advent of fast-casual Mexican-style dining establishments, such as Chipotle and Qdoba, has greatly improved the productivity of research mathematicians and theoretical
author = LaTeX with hyperref package
dbytes = 10425
fbytes = 74745
filename = burrito_monads.pdf
fmtime = 01445444345
mtime = 01445444345
mtype = application/pdf
origcharset = UTF-8
pcbytes = 74745
rcludi = /home/k/doc/books/burrito_monads.pdf|
relevancyrating = 100%
sig = 747451445444345
title = 
url = file:///home/k/doc/books/burrito_monads.pdf
```

*****

#### Search For File Names

```
➜  recoll -t -n 1 -f burrito
Recoll query: (XSFSburrito_monads.pdf)
1 results
application/pdf	[file:///home/k/doc/books/burrito_monads.pdf]	[burrito_monads.pdf]	74745	bytes	
```

## Suave.IO

> Suave is a simple web development F# library providing a lightweight web
> server and a set of combinators to manipulate route flow and task composition.

http://suave.io

*****

#### Features:

> - combinators for request routing
> - built-in web-server
> - openssl support

*****

##### The Simplest Possible Application:

``` startWebServer defaultConfig (OK "Hi.") ```

*****

##### More Elaborate Example

```
open Suave
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web

let serarch q =
  defaultArg (Option.ofChoice(q ^^ "filename")) "nothing" |> sprintf "Found %s."

let app : WebPart =
    path "/search" >>= 
      GET  >>= request(fun r -> OK <| search r.query)
      RequestErrors.NOT_FOUND "Found no handlers" ]

startWebServer defaultConfig app
```

<div class="notes">
- match GET requests on `/search` endpoint
- return a 404 for all other routes (even `/`)
</div>

*****

##### Types

```
type SuaveTask<'a> = Async<'a option>
type WebPart = HttpContext -> SuaveTask<HttpContext>
// hence: WebPart = HttpContext -> Async<HttpContext option>
```

## Pulling It All Together

We need to:

> 1) create data types (domain model)
> 2) write a simple parser for _recoll_ output
> 3) map HTTP query to command-line arguments
> 4) serialiation of results to JSON

*****

#### Dependencies

Add Suave and FParsec to _paket.depenecies_ and _src/PaperScraper/paket.references_:

```
source https://nuget.org/api/v2

nuget FSharp.Formatting
nuget NUnit 
nuget NUnit.Runners
nuget FAKE
nuget SourceLink.Fake

nuget Suave
nuget FParsec

github fsharp/FAKE modules/Octokit/Octokit.fsx
```

*****

Contents of _paket.references_:

```
Suave
FParsec
```

*****

##### A little Tooling 

```{.fsharp .fragment}
#r @"../../packages/Suave/lib/net40/Suave.dll"
#r @"../../packages/FParsec/lib/net40-client/FParsecCS.dll"
#r @"../../packages/FParsec/lib/net40-client/FParsec.dll"

#load @"Types.fs"
#load @"Recoll.fs"
```

```{.shell .fragment}
➜ cd src/PaperScraper
➜ fsharpi --load:script.fsx
```

<div class="notes">
- fsharpi is currently not aware of projects so this is userful to load the
  environment
- other than that I miss `:type` from ghci _a lot_
</div>

*****

##### Basic Types

TODO: WORK IT OUT!!!

```{.fsharp .fragment}
type MimeType = string
type FileName = string

type CharSet =
  | UTF8
  | Other // how ignorant of me ;)

type Percentage = int

type Url = string

type Bytes = int

type SearchResult =
  { Abstract  : string
  ; FileName  : FileName
  ; MimeType  : MimeType
  ; CharSet   : CharSet
  ; Relevance : Percentage
  ; Title     : string option
  ; Url       : Url
  ; FileSize  : Bytes
  }
```

*****

##### Parsing Recoll Output

TODO: insert parser code 

*****

##### Querying Recoll

TODO: insert query code

*****

##### 
