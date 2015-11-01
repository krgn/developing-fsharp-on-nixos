[<AutoOpen>]
module PaperScraper.Types

type MimeType = string
type FileName = string

type Name = string

type CharSet =
  | UTF8
  | UTF16
  | UnknownCharset of string // how ignorant of me ;)

type Percentage = int64
type QueryString = string
type Url = string

type Bytes = int64

type Row =
  { Abstract  : string
  ; FileName  : FileName
  ; MimeType  : MimeType
  ; CharSet   : CharSet
  ; Url       : Url
  }

type QueryResult =
  { Query : QueryString
  ; Count : int64
  ; Rows  : Row array
  }

  with
    static member empty =
      { Query = ""
      ; Count = 0L
      ; Rows  = Array.empty
      }
