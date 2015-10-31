[<AutoOpen>]
module PaperScraper.Types

type MimeType = string
type FileName = string

type Name = string

type CharSet =
  | UTF8
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

type BigQueryResult =
  { Query : QueryString
  ; Count : int64
  ; Rows  : Row array
  }
