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
- idea could at transferred to some more relevant technology like, e.g., elasticsearch 
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
→ export FSharpTargetsPath=$(dirname $(which fsharpc))/../lib/mono/Microsoft\ F\#/v4.0/Microsoft.FSharp.Targets
```

```{.shell .fragment}
→ set -x FSharpTargetsPath (dirname (which fsharpc))/../lib/mono/Microsoft\ F\#/v4.0/Microsoft.FSharp.Targets
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
→ set -x TargetFSharpCorePath (dirname (which fsharpc))/../lib/mono/4.0/FSharp.Core.dll
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

## But we're building an executable, right?!

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

## Recoll

> - full-text search tool 
> - uses xapian underneath (like other great tools, e.g. `notmuch` and `mu`)
> - supports many file types, including extracting text from PDFs 

<div class="notes">
</div>

## Suave.IO

TODO: give a quick overview...

Add an entry for it to `paket.dependencies` 

## Pulling Things Together
