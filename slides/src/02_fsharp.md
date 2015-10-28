# What is F\#?

#### F\#

*****

> F# was developed in 2005 at Microsoft Research[1]. In many ways, F# is
> essentially a .Net implementation of OCaml, combining the power and expressive
> syntax of functional programming with the tens of thousands of classes which
> make up the .NET class library.

*****

#### Overview

> - functional-first CLI programming language in the ML family
> - object-orientation
> - the 'm'-word (look at the computation-food paper that I downloaded)
> - ecosystem seems somewhat fragmented, but there are many useful libraries out there

<div class="notes">
</div>

*****

#### Pros: 

> - For those who (have to) write software for the CLR its a solid choice
> - With projects like WebSharper or FunScript, F# can be used throughout the whole stack (share types and code, thus safety)
> - Interop wth C# works really well, and there are lots of good libraries
> - its essentially a really good blend between the principled and utilitarian mind-sets

<div class="notes">
- WARNING: highly subjective
</div>

*****

#### Cons:

> - it introduces new nomenclature for common fp concepts (monads), to create a more clear distinction to other languages, which confused me more than it helped
> - TODO: find out differences in the type systems
> - no GADTs
> - while OO has some points to go for it (think familiarity to large audiences
>   of developers, generally well understood) it is a bit ugly and alien in this context.
 
<div class="notes">
- again, WARNING: highly subjective!
</div>

*****

#### Differences from Haskell

> - TODO: list up different operators
> - TODO: explain fixity rules
> - TODO: show a small implementation of a ComputationBuilder

> - impure: _launchMissiles ()_ whereever you feel like it
> - no `where`

*****

#### Let's look at some code

*****

#### The λ-calculus

```{.fsharp .fragment}
// variable binding
let x = 41
```

```{.fsharp .fragment}
// functions
let f = fun (value : int) -> value + 1
```

```{.fsharp .fragment}
let f value = value + 1 // shorter
```

```{.fsharp .fragment}
let f = (+) 1 // partially applied
```

```{.fsharp .fragment}
// function application
f x
```

```{.fsharp .fragment}
// a common idiom in F# is the `apply to` operator
x |> f
```

<div class="notes">
VARIABLE BINDING

- for me "seeing is believing"
- all statements are bound using `let`
- mention `let .. in` 
- talk about the type annotation 
- Not my absolute super-favourite programming language, but certainly a _very_ good one.
</div>


*****

#### Types

```{.fsharp .fragment}
// a binary tree - example of a sum type
type Tree<'a> =
  | Node of Tree<'a> * int * Tree<'a>
  | Leaf of 'a
```

```{.fsharp .fragment}
// another "discriminated union" for modeling state changes in an application
type AppAction =
  | AddThing
  | EditThing
  | RemoveThing
```

```{.fsharp .fragment}
// record - a product type
type Person = { name : string; age : int }
```

```{.fsharp .fragment}
// optional (aka. Maybe)
type option<'a> =
  | Some of 'a
  | None
```

```{.fsharp .fragment}
// type alias
type Person = (int, string)
```

*****

#### Classes

```{.fsharp}
// Objects o.O
type Person (a: int, n: string) =
  let mutable name = n
  let mutable age = a

  member self.Name          // properties
    with get () = name      // getter ->
     and set n  = name <- n // setter <-

  member self.Age
    with get () = age
     and set a  = age <- a

  member self.OldEnough () = age > 18

  static member Greet () = printfn "Hi."

// usage
let me = new Person (34, "Karsten")
me.OldEnough ()
```

<div class="notes">
- method invokation is confusing, because args are esseentially passed as a
  n-tuplet
- let-bound variables are private
- mutability is simble, if somewhat verbose
</div>

*****

#### Pattern Matching

```{.fsharp}
let horse : string option = Some "Hi."

// handling all cases with match
match f with
  | Some "Hello." -> printfn "it said hello."
  | Some "Hi."    -> printfn "it said hi."
  | Some _        -> printfn "it said something else."
  | None          -> printfn "it does not speak."
```

```{.fsharp .fragment}
TODO more examples for PM
```

<div class="notes">
- useful for producing total code
- types of pattern matching that are possible is long
</div>

*****

#### Modules and ..

```{.fsharp}
// declare a module locally
module MyTree = 
  // indent!
  type Tree<'a> =
    | Node of Tree<'a> * 'a * Tree<'a>
    | Leaf
```

```{.fsharp .fragment}
// top-level definition
module MyTree

(*
  - declare at the top of file
  - no =
  - no indentation!
*)
let testTree depth =
  let rec testTree' current max =
    let next = current + 1
    if current = (max - 1)
    then Node(Leaf, current, Leaf)
    else Node(testTree' next max, current, testTree' next max)
  testTree' 0 depth
```

*****

#### .. Namespaces

```{.fsharp .fragment}
namespace Data

module MyTree =
  // no indentation!
  type Tree<'a> =
    | Node of Tree<'a> * int * Tree<'a>
    | Leaf of 'a
```

```{.fsharp .fragment}
// combine namespace and module into one statement
module Data.MyTree

// again, no indentation!
type Tree<'a> =
  | Node of Tree<'a> * int * Tree<'a>
  | Leaf of 'a
```
*****

#### Other Cool Things To Look At

> - Monads, or _Computation Expressions_
> - Quotations & Reflection (metaprogramming)
> - Units of Measure
> - built-in support for Actor-style programming

*****

## F\# and NixOS

*****

> - F# currently is packaged separately from mono
> - as a consequence, there is no single GAC (Global Assembly Cache) for all .NET packages
> - package management is traditionally done using _nuget_ and an IDE front-end (this might make some people in the audience flinch)
> - _paket_ is a very promising replacement setting out to fix the common problem of _DLL_ hell
> - _paket_ resolves the dependency graph at the solution level and manages references of projects

*****

#### But whats the point of using Nix(OS) then?

```{.fsharp .fragment}
Tentative Answer:

because it brings a lot more value to the table than just package management!
```

*****

To use _nix_ for package management we'd need to: 

> 1) create and maintain packages for nuget packages, possibly automating the
> process with the right tooling
> 2) have a way to generate reference entries in .fsproj files automatically,
> just as _paket_ does it
> 3) build projects such that runtime deps get linked correctly

***** 

> - some work towards that end has already been done by @obadz, albeit it seems
>   experimental at this point
> - there might not be big enough incentives to do this at this point

<div class="notes">
- not trivial
- as with haskell ecosystem, not exactly easy-to-use
- haskell package management had more incentives since cabal-hell was much more
  of a pressing problem to haskell users
- disagreement?
- discussion?
- different points of view on that matter?
</div>
