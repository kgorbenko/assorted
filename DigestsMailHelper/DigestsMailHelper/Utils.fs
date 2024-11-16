module DigestsMailHelper.Utils

open System

let tryParseInt (value: string) : int option =
    match Int32.TryParse value with
    | true, int -> Some int
    | _ -> None

let toResult error option =
    match option with
    | Some value -> Ok value
    | None -> Error error