module Async

let map f m = async {
    let! x = m
    return f x
}

let bind f m = async {
    let! x = m
    return! f x
}

let lift = async.Return