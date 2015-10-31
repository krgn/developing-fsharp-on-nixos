# Hi.

<div class="notes">
Introductions:

- introduce yourself
- work @ nsynk GmbH
- my background in arts, music, computing
- interests
    * functional programming
    * systems programming
    * distributed systems
    * web
    * audio/video
- a few words about iris
    * distributed system of VVVV renderers
    * used for playback of high-quality (4K) video streams, potentially with 3d
      rendered overlays and integration with all kinds of other systems
      (sensors, kinetics etc)
    * IAA 
- my usage of NixOS
    * mainly as a development platform
    * use VMs for Windows-based work
</div>

# Why? Who? What?

- I am not a Nix(OS) or F\# expert or .NET veteran, yet.
- this talk is aimed at people with experience level roughly to my own
- sharing my personal experience and impressions
- it is my first talk, so please criticize me (gently :))

<div class="notes">
</div>

# What is F\# (FSharp)?

Not my absolute super-favourite programming language, but certainly a _very_ good one.

<div class="notes">
</div>

## Quick Overview

- functional-first CLI programming language in the ML family
- object-orientation
- the 'm'-word (look at the computation-food paper that I downloaded)
- ecosystem seems somewhat fragmented, but there are many useful libraries out there

<div class="notes">
</div>

## Pros: 

- For those who (have to) write software for the CLR its a solid choice
- With projects like WebSharper or FunScript, F# can be used throughout the
  whole stack (share types and code, thus safety)
- Interop wth C# works really well, and there are lots of good libraries
- its essentially a really good blend between the principled and utilitarian
  mind-sets

<div class="notes">
</div>

## Cons:

- it introduces new nomenclature for common fp concepts (monads), to create a
  more clear distinction to other languages, which confused me more than it
  helped
- TODO: find out differences in the type systems
- while OO is nice because of its familiarity to people like myself, its a bit
  ugly and alien in this context.
 
<div class="notes">
</div>

## Developing F\# on NixOS

- F# packaged separately from mono
- as a consequence, there is no single GAC (Global Assembly Cache) for all
  required `dll`s
- package management is traditionally done using `nuget`, and now `paket` which
  might make some of you in the audience flinch
- because, you know, wasn't the whole point of using `nix` to solve these kinds
  of problems?
- since we'd need to
  1) create and maintain packages for all nuget packages and
  2) have a way to generate reference entries in .fsproj files
  there might not be a big incentive at this point to go about and re-invent the
  wheel at this point.

<div class="notes">
</div>

# Microservices

TODO: remind myself WTF are they again?

No, but seriously, its a stupid buzzword and we all know it but apparently we
like pressing those buttons over and over again.

(Ren and Stimpy buttons picture where they look kinda mad)

A microservice is a small, stand-alone component most often part of a system of
more of these stand-alone, de-coupled units of computation. Its a scalability
design pattern for distributed applications.

It is also, from the point of view of a functional programming enthusiast a good
example of how small, stateless (pure!) building blocks (functions) can be
_composed_ into systems that are easier to understand and maintain and more
robust that big monolithic code-bases.

I am not a dev-ops person, but I hear that larger systems become fiendishly hard
to deploy and monitor, though. Shift the blame... :)

<div class="notes">
</div>

# Introducing the project idea

We have a great many computer science papers from some nice peoples repositories
we want to be able to index and query for information (e.g. [1]).

The idea is to build a small API around the recoll full-text indexer.

[1] cocharles' git-annex repo

<div class="notes">
</div>

# Bootstrapping a new project

The state of affairs of project management in mono/F# is still largely centered
around using IDE's for everything, which is a bit of an annoyance.

To alleviate that situation there is a project scaffold git repo to help set up
everything.

We clone that repo and rename it `PaperTrail`, the name of our little service.

```
→ git clone git@github.com:fsprojects/ProjectScaffold.git PaperTrail
→ cd PaperTrail
→ rm -rf .git
→ git init
→ ./build.sh
```
Answer a bunch of questions and you're set. Wait! But whats this!?!

```
/home/k/src/projects/PaperTrail/src/PaperTrail/PaperTrail.fsproj: error : Target named 'Rebuild' not found in the project.
```

Argh! (TODO: add a batman photo of "POW!" or something)

```
→ nix-env -i dotnetbuildhelpers
```

```
→ patch-fsharp-targets.sh
  Patching F# targets in fsproj files...
  ./src/PaperTrail/PaperTrail.fsproj
  ./tests/PaperTrail.Tests/PaperTrail.Tests.fsproj
```


in scripts/.bashrc etc:

```
→ export FSharpTargetsPath=$(dirname $(which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets
or
→ set -x FSharpTargetsPath (dirname (which fsharpc))/../lib/mono/4.5/Microsoft.FSharp.Targets
```

Add an entry for it to `paket.dependencies` 

<div class="notes">
</div>

# Suave.IO

TODO: give a quick overview...

Add an entry for it to `paket.dependencies` 

# Recoll

- full-text search tool 
- uses xapian underneath (like other great tools, e.g. `notmuch` and `mu`)
- supports many file types, including extracting text from PDFs 

<div class="notes">
</div>

# Writing a derivation

```
→ export FSharpTargetsPath="${fsharp}/lib/mono/4.5/Microsoft.FSharp.Targets"
```

<div class="notes">
</div>

# Systemd services

- need a service for the api server
- need a timer and service for recollindexer

<div class="notes">
</div>

# Deployment

- sending a closure
- container?

<div class="notes">
</div>

# Trying it out

<div class="notes">
</div>

## Useful resources

- fsharpforfunandprofit blog thingy
- FSharp WikiBook
- M$ language reference

<div class="notes">
</div>
