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

Since in _NixOS_ there is not Global Assembly Cache, the standard mechanisms for
resolving default build targets shipped with _F#_ does not work as expected.

*****

The solution is to add a secion to all _.fsproj_ files in the solution to either
hard-code the path or use an environment variable.

*****

```→ nix-env -i dotnetbuildhelpers```

*****

> → patch-fsharp-targets.sh
>   Patching F# targets in fsproj files...
>   ./src/PaperTrail/PaperTrail.fsproj
>   ./tests/PaperTrail.Tests/PaperTrail.Tests.fsproj

in scripts/.bashrc etc:

> → export FSharpTargetsPath=$(dirname $(which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets
> or
> → set -x FSharpTargetsPath (dirname (which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets

Add an entry for it to `paket.dependencies` 

<div class="notes">
</div>

## But we're building an executable, right?!

# Suave.IO

TODO: give a quick overview...

Add an entry for it to `paket.dependencies` 

## Recoll

> - full-text search tool 
> - uses xapian underneath (like other great tools, e.g. `notmuch` and `mu`)
> - supports many file types, including extracting text from PDFs 

<div class="notes">
</div>
