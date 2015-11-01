[<AutoOpen>]
module PaperScraper.Types

open System.Runtime.Serialization

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

[<DataContract>]
type Row =
  {
  [<field: DataMember(Name = "abstract")>]
  Abstract : string;

  [<field: DataMember(Name = "filename")>]
  FileName : FileName;

  [<field: DataMember(Name = "mimetype")>]
  MimeType : MimeType;

  [<field: DataMember(Name = "charset")>]
  CharSet : CharSet;

  [<field: DataMember(Name = "url")>]
  Url : Url;
  }

[<DataContract>]
type QueryResult =
  {
  [<field: DataMember(Name = "query")>]
  Query : QueryString;

  [<field: DataMember(Name = "count")>]
  Count : int64;

  [<field: DataMember(Name = "rows")>]
  Rows : Row array;
  }

  with
    static member empty =
      { Query = ""
      ; Count = 0L
      ; Rows  = Array.empty
      }
