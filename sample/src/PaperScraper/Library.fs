namespace PaperScraper

(*
recollq: usage:
 -P: Show the date span for all the documents present in the index
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

/// Documentation for my library
///
/// ## Example
///
///     let h = Library.hello 1
///     printfn "%d" h
///
module Library = 
  
  /// Returns 42
  ///
  /// ## Parameters
  ///  - `num` - whatever
  let hello num = 42
