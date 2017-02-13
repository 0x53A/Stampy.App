module Messages

open System

[<RequireQualifiedAccess>]
type Page =
| Home
| Overview

type AppMsg =
| NavigateTo of Page
| NavigateBack
| ExitApp
| HomeSceneMsg of HomeSceneMsg
| OverviewMsg of OverviewMsg

and [<RequireQualifiedAccess>] HomeSceneMsg =
| LoadData
| LoadedData of Model.LoginInformation option
| Connect
| Connected of string
| ChangeUsername of string
| ChangePassword of string
| Error of exn

and [<RequireQualifiedAccess>] OverviewMsg =
| LoadData
| LoadedData of Model.StampRecord[]
| Stamp of Model.StampType
| StampConfirmedByServer of Model.StampRecord
| Logout
| Error of exn