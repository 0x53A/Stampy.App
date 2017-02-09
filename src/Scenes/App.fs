module App

open System
open Fable.Core
open Fable.Import
open Fable.Import.ReactNative
open Fable.Helpers.ReactNative
open Fable.Helpers.ReactNative.Props
open Fable.Import.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal
open Fable.Helpers.ReactNativeOneSignal.Props
open Elmish
open Elmish.React
open Elmish.ReactNative
open Messages
open Fable.Core.JsInterop

type SubModel =
| HomeModel of Home.Model
| OverviewModel of Overview.Model

type AppModel = {
    SubModel : SubModel
    NavigationStack: Page list
}

let wrap ctor model (subModel,cmd)  =
    { model with SubModel = ctor subModel },cmd

let navigateTo page newStack model =
    match page with
    | Page.Overview -> Overview.init() |> wrap OverviewModel model
    | Page.Home -> Home.init() |> wrap HomeModel model
    |> fun (model,cmd) -> { model with NavigationStack = newStack },cmd

let update (msg:AppMsg) model : AppModel*Cmd<AppMsg> = 
    match msg with
    | HomeSceneMsg subMsg ->
        match model.SubModel with
        | HomeModel subModel -> Home.update subMsg subModel |> wrap HomeModel model
        | _ -> model,Cmd.none
    | OverviewMsg subMsg ->
        match model.SubModel with
        | OverviewModel subModel -> Overview.update subMsg subModel |> wrap OverviewModel model
        | _ -> model,Cmd.none
    | NavigateTo page -> navigateTo page (page::model.NavigationStack) model
    | NavigateBack -> 
        match model.NavigationStack with
        | _::page::rest -> navigateTo page (page::rest) model
        | _ -> model,Cmd.ofMsg ExitApp
    | ExitApp -> 
        Fable.Helpers.ReactNative.exitApp() 
        model,Cmd.none

let init() =
    let subModel,cmd = Home.init() 
    { SubModel = HomeModel subModel
      NavigationStack = [Page.Home] }, cmd

let view (model:AppModel) (dispatch: AppMsg -> unit) =
    match model.SubModel with
    | HomeModel model -> lazyView2 Home.view model dispatch
    | OverviewModel model -> lazyView2 Overview.view model dispatch

let setupBackHandler dispatch =    
    let backHandler () =
        dispatch AppMsg.NavigateBack
        true

    Fable.Helpers.ReactNative.setOnHardwareBackPressHandler backHandler

let subscribe (model:AppModel) =
    Cmd.batch [
        Cmd.ofSub setupBackHandler ]