module Retry

open System
open System.Net.Http
open OptionOperators

type Request = { Value : int }
type Response = { Value :  int }

let send (s : string) = 10 |> string

let catchTimeout f v =
    try Some (f v) with :? TimeoutException -> None

let serializeRequest (r : Request) = r.Value |> string
let parseResponse (s : string) =
    match Int32.TryParse(s) with
    | true, v  -> Some v
    | false, _ -> None

let rec retry retries (f : 'a -> 'b option) a =
    if retries >= 0 then
        match f a with
        | Some v -> Some v    
        | None   -> retry (retries - 1) f a
    else
        None

let doIt =
    serializeRequest
    >> retry 5 (catchTimeout send) >=> parseResponse

let createUndoRequest = id
let undoIt (r : Request) =
    r 
    |> createUndoRequest
    |> serializeRequest
    |> (retry 5 (catchTimeout send) >=> parseResponse)

let ``butIfItFails`` action compensation input =
    match action input with
    | Some v -> Some v
    | None -> compensation input

let action = doIt |> ``butIfItFails`` <| undoIt

let actionWithRetry = retry 3 action

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    0 // return an integer exit code
