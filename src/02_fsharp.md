# What is F\#?

#### F\#

*****

> - developed in 2005 at Microsoft Research
> - essentially a .NET implementation of OCaml
> - functional-first, strict, multi-paradigm programming language
> - first-class citizen in the .NET ecosystem
> - compiles to IL (byte-code format) and runs on CLR (AOT available)

<div class="notes">
* type system is considerably simpler than haskells
* no type classes
</div>

*****

#### Ecosystem

> - Free Software!
> - runs on mono, as well as the coreclr (both available through nix)
> - feeds off of Nuget (package manager and repository)

<div class="notes">
- has a host of good libraries
</div>

*****

#### Pros: 

> - great integration with the CLR and other languages targeting it
> - lots of good libraries
> - full-stack language 
> - good blend of principled and pragmatic mind-sets
> - reflection & dynamic typing

<div class="notes">
- WARNING: highly subjective
- WebSharper
- Akka.NET
- MBrace 
</div>

*****

#### Cons:

> - no type-classes
> - noisy syntax
> - quirky syntax
> - impure
> - introduces new (confusing) nomenclature for common fp concepts
> - reflection & dynamic typing
 
<div class="notes">
- again, WARNING: highly subjective!
- quirks include method call syntax tuples?
- also, methods can apparently not easly be partially applied
- class constructors cannot be partially applied
- lots of little inconsistencies
- type checker is finicky
- lots of braces in interaction with classes
</div>

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

- "seeing is believing"
- all statements are bound using `let` (noise)
- mention `let .. in` 
- talk about the type annotation 
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
- method invokation is confusing
- args are esseentially passed as a n-tuples
- let-bound variables are private
- mutability is simple, if somewhat verbose
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

> - Type Providers!
> - Monads, or _Computation Expressions_
> - Quotations & Reflection (metaprogramming)
> - Units of Measure
> - MailboxProessors

<div class="notes">
* type providers cool to integrate with APIs
* Async good support for concurrent and parallel computation (Async.Parallel)
</div>

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
