module Model

open System

[<RequireQualifiedAccess>]
type LocationStatus =
| Ok
| Alarm of string

type LoginInformation = {
    Username : string
    Password : string
}

[<RequireQualifiedAccess>]
type StampType = Login | Logout

type StampRecord = { StampType : StampType ; Timestamp : DateTime }
