module Retry

open System
open System.Net.Http

//let client = new HttpClient();
// let send (request : HttpRequestMessage) = async { return new HttpResponseMessage() }
// let readResponse (r : HttpResponseMessage) = r.Content.ReadAsStringAsync() |> Async.AwaitTask

type Request = { Value : int }

type Response = { Value :  int }

let send (s : string) = 10 |> string

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
    >> retry 5 (send >> parseResponse)

[<EntryPoint>]
let main argv =
    printfn "%A" argv
    0 // return an integer exit code
