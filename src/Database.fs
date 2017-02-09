module Database

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Helpers.ReactNativeSimpleStore
open Fable.PowerPack
open Model

let getLastLogin () = promise {
    let! records = DB.getAll<Model.LoginInformation>()
    return records |> Seq.tryPick Some
}

let setLastLogin (ll) = promise {
    do! DB.clear<Model.LoginInformation>()
    do! DB.add<Model.LoginInformation> ll
}

let getStampHistory () = promise {
    let! x = DB.getAll<Model.StampRecord>()
    return x |> Seq.sortBy (fun sr -> sr.Timestamp)
}

let addStamp sr = DB.add<Model.StampRecord> sr