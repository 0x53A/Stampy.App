module Overview

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Import.ReactNativeImagePicker
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.PowerPack
open Fable.Core.JsInterop
open Elmish
open Messages

// Model

type Status =
| Unknown
| CheckedIn
| CheckedOut

type Model =
  { StatusText : string
    Status : Status
  }

let init () =
    { Status = Unknown
      StatusText = "..."}, Cmd.ofMsg OverviewMsg.LoadData |> Cmd.map OverviewMsg

// Update
let update msg model : Model*Cmd<AppMsg> =
    match msg with
    | OverviewMsg.LoadData ->
        let model2 = { model with StatusText = "Loading ..."}
        let prData = fun _ ->
            Fetch.fetchAs<Model.StampRecord[]> "http://..." []
        model2, Cmd.ofPromise prData () OverviewMsg.LoadedData OverviewMsg.Error |> Cmd.map OverviewMsg
    | OverviewMsg.LoadedData data ->
        { model with StatusText = "Loaded Data" }, Cmd.none
    | OverviewMsg.Stamp stamp ->
        let model2 = { model with StatusText = "Sending ..." }
        let prData = fun _ -> promise {  
            let! r = Fetch.postRecord "http:// ..." stamp []
            let! text = r.text()
            let dt = text |> ofJson<DateTime>
            return ({ StampType = stamp ; Timestamp = dt }:Model.StampRecord)
        }
        model2, Cmd.ofPromise prData () OverviewMsg.StampConfirmedByServer OverviewMsg.Error |> Cmd.map OverviewMsg
    | OverviewMsg.StampConfirmedByServer stamp -> model, Cmd.ofMsg (AppMsg.NavigateTo Page.Home)
    | OverviewMsg.Logout -> model, Cmd.ofMsg (AppMsg.NavigateTo Page.Home)
    | OverviewMsg.Error e ->
        { model with StatusText = e.Message }, Cmd.none

// View
let view (model:Model) (dispatch: AppMsg -> unit) =

    view [ Styles.sceneBackground ]
        [ text [ Styles.titleText ] "Hello"
          text [ Styles.defaultText ] model.StatusText
          Styles.button "Come" (fun () -> OverviewMsg.Stamp Model.StampType.Login
                                          |> AppMsg.OverviewMsg |> dispatch )
          Styles.button "Go" (fun () -> OverviewMsg.Stamp Model.StampType.Logout
                                        |> AppMsg.OverviewMsg |> dispatch )
        ]