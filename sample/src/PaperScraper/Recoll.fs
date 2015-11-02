[<AutoOpen>]
module PaperScraper.Recoll

open System
open FParsec

let consume = true

let plainChar (c : char) = 
  let chars = ['a'..'z']
  let pred c' = c' = c 
  match List.tryFind pred chars with
    | Some(_) -> true
    | _       -> false

let skipLine : Parser<string,unit> = restOfLine consume

let plainChars : Parser<string, unit> = 
  manySatisfy plainChar

let mimeType : Parser<string, unit> =
  plainChars  >>= fun res1 ->
  pstring "/" >>= fun _    ->
  plainChars  >>= fun res2 ->
  preturn (res1+"/"+res2)

let fileUrl : Parser<string,unit> =
  pstring "file:///" >>= fun b ->
    restOfLine consume >>= fun e -> preturn (b + e)

let queryLine : Parser<string, unit> =
  pstring "Recoll query:" >>. restOfLine consume

let countLine : Parser<int64, unit> =
  skipManyTill anyChar (pstring "printing")
  >>. spaces
  >>. pint64
  .>> skipRestOfLine consume

let totalLine : Parser<int64, unit> =
  pint64 .>> skipRestOfLine consume

let abstractLine : Parser<string, unit> =
  pstring "abstract = " >>. restOfLine consume

let titleLine : Parser<Name, unit> =
  pstring "title = " >>. restOfLine consume

let filenameLine : Parser<FileName, unit> =
  pstring "filename = " >>. restOfLine consume
  
let mtypeLine : Parser<MimeType, unit> =
  pstring "mtype = " >>. mimeType .>> skipRestOfLine consume

let (|CharSetLit|_|) (prefix : string) (str : string) =
  if str.StartsWith(prefix)
  then Some(str.Substring(prefix.Length))
  else None

let charsetLine : Parser<CharSet,unit> =
  pstring "origcharset = " >>. 
  restOfLine consume >>= fun str ->
    match str with
      | CharSetLit "UTF-8" _  -> UTF8
      | CharSetLit "UTF-16" _ -> UTF16
      | s                     -> UnknownCharset s
    |> preturn
    .>> skipRestOfLine consume

let urlLine : Parser<Url, unit> =
  pstring "url = " >>. restOfLine consume

let skipTo p =
  skipManyTill (restOfLine true) (lookAhead p) >>. p

let mkRow a f m c u =
  { Abstract = a
  ; FileName = f
  ; MimeType = m
  ; CharSet  = c
  ; Url      = u }

let searchResult : Parser<Row,unit> =
  pipe5 (skipTo abstractLine)
        (skipTo filenameLine)
        (skipTo mtypeLine)
        (skipTo charsetLine)
        (skipTo urlLine)
        mkRow

let mkResult query count results =
  { Query = query
  ; Count = count
  ; Rows  = results
  }

let recollOutput : Parser<QueryResult, unit> =
  queryLine >>= fun q ->
  totalLine >>= fun c ->
  (parray (int(c)) searchResult) >>= fun rows ->
  preturn (mkResult q c rows)

let parseOutput str =
  match run recollOutput str with
    | Success(res, _, _) -> res
    | Failure(msg, _, _) -> failwith msg

let executeProcess (exe,cmdline) =
  let psi = new System.Diagnostics.ProcessStartInfo(exe,cmdline) 
  psi.UseShellExecute <- false
  psi.RedirectStandardOutput <- true
  let p = System.Diagnostics.Process.Start(psi) 
  let out = p.StandardOutput.ReadToEnd() 
  p.WaitForExit()
  ( p.ExitCode, out )


let queryRecoll term =
  let binpath = "/run/current-system/sw/bin/recoll"
  let raw = executeProcess (binpath, sprintf "-t -o -m -q %s" term)
  if fst raw = 0
  then parseOutput (snd raw)
  else failwith "running recoll failed"
