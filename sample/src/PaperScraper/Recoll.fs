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

open FParsec

let plainChar (c : char) = 
  let chars = ['a'..'z']
  let pred c' = c' = c 
  match List.tryFind pred chars with
    | Some(_) -> true
    | _       -> false

let plainChars : Parser<string, unit> = 
  manySatisfy plainChar

let mimeType : Parser<string, unit> =
  plainChars >>= fun res1 ->
    pstring "/" >>= fun _ ->
      plainChars >>= fun res2 ->
        spaces >>= fun _ -> 
          preturn (res1+"/"+res2)


let fileUrl : string = failwith "FIXME"
let title : string = failwith "FIXME"
let charSet : string = failwith "FIXME"


          
let test p str =
  match run p str with
    | Success(result, _, _) -> printfn "Success: %s" result
    | Failure(msg, _, _)    -> printfn "Failure: %s" msg

let testInput =
  @"
Recoll query: ((monad:(wqf=11) OR monads OR monadic OR monadically OR monadicity OR monadized))
1519 results (printing  2 max):
application/pdf	[file:///home/k/doc/books/cocharles-paper/A Poor Man's Concurrency Monad.pdf]	[A Poor Man's Concurrency Monad.pdf]	196433	bytes	
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
