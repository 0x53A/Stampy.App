module Home

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.PowerPack
open Elmish
open Messages

// Model
type Model = {
    StatusText : string
    Username : string
    Password : string 
}

// Update
let update (msg:HomeSceneMsg) model : Model*Cmd<AppMsg> =
    match msg with
    | HomeSceneMsg.ChangeUsername u -> { model with Username = u }, []
    | HomeSceneMsg.ChangePassword p -> { model with Password = p }, []
    | HomeSceneMsg.LoadData ->
        { model with StatusText = "Loading previous Login information ..." },
        Cmd.ofPromise Database.getLastLogin () HomeSceneMsg.LoadedData HomeSceneMsg.Error
        |> Cmd.map HomeSceneMsg
    | HomeSceneMsg.LoadedData (prevData) ->
        match prevData with
        | None -> { model with StatusText = "Please enter Login information" }, []
        | Some d ->
            { model with Username = d.Username ; Password = d.Password }, []
    | HomeSceneMsg.Connect ->
        let connectRequest = fun _ -> promise {
            do! Database.setLastLogin { Username = model.Username ; Password = model.Password }
            let! response = Fetch.postRecord "" { Username = model.Username ; Password = model.Password } []
            let! text = response.text()
            return text
        }
        { model with StatusText = "Connecting ..." },
        Cmd.ofPromise connectRequest () HomeSceneMsg.Connected HomeSceneMsg.Error
        |> Cmd.map HomeSceneMsg
    | HomeSceneMsg.Connected token ->
        { model with StatusText = "Connected!" },
        Cmd.ofMsg (AppMsg.NavigateTo Page.Overview)
    | HomeSceneMsg.Error e ->
        { model with StatusText = string e.Message }, []


let init () = { StatusText = "" ; Username = "lrieger" ; Password = "pw12345" }, Cmd.ofMsg HomeSceneMsg.LoadData |> Cmd.map HomeSceneMsg

// View
let view (model:Model) (dispatch: AppMsg -> unit) =

      view [ Styles.sceneBackground ]
        [ text [ Styles.titleText ] "Header"
          view [] [ text [] "Username" ; textInput [ TextInput.Value(model.Username) ; TextInput.OnChangeText (fun s -> dispatch (HomeSceneMsg (HomeSceneMsg.ChangeUsername s))) ] null ]
          view [] [ text [] "Password" ; textInput [ TextInput.Value(model.Password) ; TextInput.OnChangeText (fun s -> dispatch (HomeSceneMsg (HomeSceneMsg.ChangePassword s))) ] null ]
          Styles.button "Connect" (fun () -> dispatch (HomeSceneMsg (HomeSceneMsg.Connect)))
          Styles.whitespace
          Styles.whitespace
          text [ Styles.smallText ] model.StatusText  ]
