module PaperScraper.Main

open Suave
open Suave.Types
open Suave.Json
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Web

open PaperScraper.Types
open PaperScraper.Recoll

(*
   Whitespace rules in F# are more finicky in my experience (fun -> indenation)

   Documentation for Suave.IO is somewhat out of date or confusing such that
   reading sources is often only way to find/understand an app.

   Your web parts are “values” in the sense that they evaluate once, e.g. when
   constructing choose [ OK "hi" ], OK "hi" is evaluated once, not every request.

   also need something about logging with resp. to systemd
*)

let proc (req : HttpRequest) =
  printfn "%s" <| (req.query).ToString()
  match (List.tryFind (fst >> (=) "term") req.query) with
    | Some(_, v) ->
      match v with
        | Some term -> queryRecoll term
        | None -> QueryResult.empty
    | None -> QueryResult.empty
  |> toJson
  |> ok


let start = startWebServer defaultConfig

let respond =
  Writers.setMimeType "application/json" >>= request(proc)

let app =
  choose [ GET >>= path "/search" >>= respond 
           NOT_FOUND "Resource not found." ]

[<EntryPoint>]
let main _ =
  start app
  0
