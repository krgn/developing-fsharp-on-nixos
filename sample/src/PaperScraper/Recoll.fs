[<AutoOpen>]
module PaperScraper.Recoll

(*
recollq: usage:
 -P: Shw the date span for all the documents present in the index
 [-o|-a|-f] [-q] <query string>
 Runs a recoll query and displays result lines. 
  Default: will interpret the argument(s) as a xesam query string
    query may be like: 
    implicit AND, Exclusion, field spec:    t1 -t2 title:t3
    OR has priority: t1 OR t2 t3 OR t4 means (t1 OR t2) AND (t3 OR t4)
    Phrase: "t1 t2" (needs additional quoting on cmd line)
  -o Emulate the GUI simple search in ANY TERM mode
  -a Emulate the GUI simple search in ALL TERMS mode
  -f Emulate the GUI simple search in filename mode
  -q is just ignored (compatibility with the recoll GUI command line)
Common options:
    -c <configdir> : specify config directory, overriding $RECOLL_CONFDIR
    -d also dump file contents
    -n [first-]<cnt> define the result slice. The default value for [first]
       is 0. Without the option, the default max count is 2000.
       Use n=0 for no limit
    -b : basic. Just output urls, no mime types or titles
    -Q : no result lines, just the processed query and result count
    -m : dump the whole document meta[] array for each result
    -A : output the document abstracts
    -S fld : sort by field <fld>
    -s stemlang : set stemming language to use (must exist in index...)
       Use -s "" to turn off stem expansion
    -D : sort descending
    -i <dbdir> : additional index, several can be given
    -e use url encoding (%xx) for urls
    -F <field name list> : output exactly these fields for each result.
       The field values are encoded in base64, output in one line and 
       separated by one space character. This is the recommended format 
       for use by other programs. Use a normal query with option -m to 
       see the field names.
*)

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
  plainChars >>= fun res1 ->
    pstring "/" >>= fun _ ->
      plainChars >>= fun res2 ->
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

let charsetLine : Parser<CharSet,unit> =
  pstring "origcharset = " >>. 
  restOfLine consume >>= fun str ->
    match str with
      | "UTF-8" -> UTF8
      | s       -> UnknownCharset s
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

// let searchResult : Parser<SearchResult, unit> =
//   skipLine >>.  // header line
//     abstractLine >>= fun abstrct ->
//       skipManyTill skipLine
//       <| filenameLine >>= fun fname -> 
//         skipManyTill skipLine
//         <| mtypeLine >>= fun mime -> 
//              charsetLine >>= fun charset ->
//                skipManyTill skipLine
//                <| urlLine >>= fun url ->
//                     preturn { Abstract = abstrct
//                             ; FileName = fname
//                             ; MimeType = mime
//                             ; CharSet = charset
//                             ; Url = url
//                             }
//                       .>> skipRestOfLine consume
      
let mkResult query count results =
  { Query = query
  ; Count = count
  ; Rows  = results
  }

let recollOutput : Parser<BigQueryResult, unit> =
   queryLine >>= fun q ->
   totalLine >>= fun c ->
   (parray (int(c)) searchResult) >>= fun rows ->
   preturn (mkResult q c rows)

let parseOutput str =
  match run recollOutput str with
    | Success(res, _, _) -> res
    | Failure(msg, _, _) -> failwith msg

let testInput1 =
  @"
Recoll query: ((monad:(wqf=11) OR monads OR monadic OR monadically OR monadicity OR monadized))
1 results (printing  1 max):
text/plain	[file:///home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs]	[Recoll.fs]	4890	bytes	
abstract = [<AutoOpen>] module PaperScraper.Recoll (* recollq: usage:  -P: Shw the date span for all the documents present in the index  [-o|-a|-f] [-q] <query string>  Runs a recoll query and displays result lines.    Default: will interpret the argument(s)
dbytes = 4890
fbytes = 4890
filename = Recoll.fs
fmtime = 01446155658
mtime = 01446155658
mtype = text/plain
origcharset = UTF-8
pcbytes = 4890
rcludi = /home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs|
relevancyrating = 100%
sig = 48901446155658
title = 
url = file:///home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs
  "

let testInput2 =
  @"
Recoll query: ((monad:(wqf=11) OR monads OR monadic OR monadically OR monadicity OR monadized))
2 results (printing  2 max):
application/pdf	[file:///home/k/doc/books/cocharles-paper/A Poor Man's Concurrency Monad.pdf]	[A Poor Man's Concurrency Monad.pdf]	196434	bytes	
abstract =   c 1993 Cambridge University Press J. Functional Programming 1 (1): 1{000, January 1993  1 FUNCTIONAL PEARLS A Poor Man's Concurrency Monad Koen Claessen Chalmers University of Technology email: koen@cs.chalmers.se Abstract Without adding any
dbytes = 21217
fbytes = 196433
filename = A Poor Man's Concurrency Monad.pdf
fmtime = 01401924059
mtime = 01401924059
mtype = application/pdf
origcharset = UTF-8
pcbytes = 196433
rcludi = /home/k/doc/books/cocharles-paper/A Poor Man's Concurrency Monad.pdf|
relevancyrating =  66%
sig = 1964331444236888
title = 
url = file:///home/k/doc/books/cocharles-paper/A Poor Man's Concurrency Monad.pdf
application/pdf	[file:///home/k/doc/books/fp/iterator.pdf]	[iterator.dvi]	173774	bytes	
abstract =   Under consideration for publication in J. Functional Programming 1 The Essence of the Iterator Pattern Jeremy Gibbons and Bruno C. d. S. Oliveira Oxford University Computing Laboratory Wolfson Building, Parks Road, Oxford OX1 3QD, UK http://www
author = dvips(k) 5.96 Copyright 2005 Radical Eye Software
caption = iterator.dvi
dbytes = 66981
fbytes = 173774
filename = iterator.pdf
fmtime = 01436217058
mtime = 01436217058
mtype = application/pdf
origcharset = UTF-8
pcbytes = 173774
rcludi = /home/k/doc/books/fp/iterator.pdf|
relevancyrating =  66%
sig = 1737741444416662
title = iterator.dvi
url = file:///home/k/doc/books/fp/iterator.pdf
  "

let testInput3 =
  @"
Recoll query: ((monad:(wqf=11) OR monads OR monadic OR monadically OR monadicity OR monadized))
1 results (printing  1 max):
text/plain	[file:///home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs]	[Recoll.fs]	4890	bytes	
abstract = [<AutoOpen>] module PaperScraper.Recoll (* recollq: usage:  -P: Shw the date span for all the documents present in the index  [-o|-a|-f] [-q] <query string>  Runs a recoll query and displays result lines.    Default: will interpret the argument(s)
dbytes = 4890
fbytes = 4890
filename = Recoll.fs
fmtime = 01446155658
mtime = 01446155658
mtype = text/plain
origcharset = UTF-8
pcbytes = 4890
rcludi = /home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs|
relevancyrating = 100%
sig = 48901446155658
title = 
url = file:///home/k/doc/nixconf/sample/src/PaperScraper/Recoll.fs
  "

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
  else failwith "command failed"
