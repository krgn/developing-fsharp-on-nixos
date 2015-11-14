# What is F\#?

*****

> - developed in 2005 at Microsoft Research
> - essentially a _.NET_ implementation of OCaml
> - functional-first, strict, multi-paradigm programming language
> - first-class citizen in the _.NET_ ecosystem
> - compiles to byte-code and runs on _CLR_
> - _AOT_ available in _mono_

<div class="notes">
* type system is considerably simpler than haskells
* no type classes
</div>

*****

#### Ecosystem

> - Free Software!
> - runs on _mono_, as well as the _coreclr_ (both in nix)
> - feeds off of Nuget (package manager and repository)

<div class="notes">
- has a host of good libraries
</div>

*****

#### Pros: 

> - great integration with other languages targeting CLR 
> - lots of good libraries
> - full-stack language 
> - good blend of principled *and* pragmatic mind-sets

<div class="notes">
- reflection & dynamic typing
- WARNING: highly subjective
- WebSharper
- Akka.NET
- MBrace 
</div>

*****

#### Cons:

> - no type-classes
> - noisy syntax & sometimes quirky syntax
> - introduces new (confusing) nomenclature
> - impure
 
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

![](img/luis.png)

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
match horse with
  | Some "Hello." -> printfn "it said hello."
  | Some "Hi."    -> printfn "it said hi."
  | Some _        -> printfn "it said something else."
  | None          -> printfn "it does not speak."
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
> - MailboxProcessors
> - Async

<div class="notes">
* type providers cool to integrate with APIs
* Async good support for concurrent and parallel computation (Async.Parallel)
</div>
