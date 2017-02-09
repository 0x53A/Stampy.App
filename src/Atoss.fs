module Atoss

open System
open Fable.PowerPack
open Fable.PowerPack.Result

type Mode = Login | Logout


let findBetweenAfterI (text:string) (i:int) (beginExcl:string) (endExcl:string) =

    let indexOfBegin = text.IndexOf(beginExcl, i)
    if indexOfBegin < 0 then failwith "could not find begin"
    let startIndex = indexOfBegin + beginExcl.Length
    let indexOfEnd = text.IndexOf(endExcl, startIndex = startIndex)
    if indexOfEnd < 0 then failwith "could not find end"
    let value = text.Substring(startIndex, indexOfEnd - startIndex)
    value
    
let findBetweenAfter (text:string) (after:string) (beginExcl:string) (endExcl:string) =
    let indexOfAfter = text.IndexOf(after)
    if indexOfAfter < 0 then failwith "could not find after"
    findBetweenAfterI text (indexOfAfter + 1) beginExcl endExcl

let findBetween (text:string) (beginExcl:string) (endExcl:string) =
    findBetweenAfterI text 0 beginExcl endExcl

let findBuchungsID mode (body:string) =
    let textToLookFor =
        match mode with
        | Login -> "value:'Kommen'"
        | Logout -> "value:'Gehen'"
    let beforeArr = body.Split([|textToLookFor|], StringSplitOptions.None)
    if beforeArr.Length <> 2 then failwithf "Expected 2, got %i" beforeArr.Length
    let before = beforeArr.[0]
    let split2Arr = before.Split([|"['zul.sel.Option',"|], StringSplitOptions.None)
    let xxx = split2Arr |> Seq.last
    let value = findBetween xxx "'" "'"
    value

let getASD () = promise {
    let! r = Fetch.tryFetch "asd" [ Fable.PowerPack.Fetch.Fetch_types.RequestProperties.Headers [] ]
    match r with
    | Ok response ->
        let headers = response.Headers
        ()
    | Error xn -> ()
    return ()
}

// let run(userId, password, modeOpt:Mode option) = task {
//     let baseAddress = "https://timecontrol.nemetschek.com"
//     let cookies = CookieContainer()
//     use handler = new HttpClientHandler(CookieContainer = cookies)
//     use client = new HttpClient(handler, BaseAddress = Uri baseAddress)

//     // get web page
//     let! r = client.GetAsync(baseAddress + "/SES/web")
//     if not r.IsSuccessStatusCode then
//         failwithf "initial request failed"
        
//     let! loginPageBody = r.Content.ReadAsStringAsync()
// //    printfn "BODY:\n\n%s\n\n==========================================================" loginPageBody
// //    for c : Cookie in cookies.GetCookies(Uri ( baseAddress + "/SES/")) |> Enumerable.Cast do
// //        printfn "%s: %s" c.Name c.Value
    
//     let sessionIdC : Cookie = cookies.GetCookies(Uri ( baseAddress + "/SES/")) |> Enumerable.Cast |> Seq.exactlyOne
//     let sessionId = sessionIdC.Value
    
//     let dtid = findBetween loginPageBody "{dt:'" "'"

//     // now login (these values are copied from fiddler)
//     use formContent =
//         let idOfPrecastAt = findBetweenAfter loginPageBody "label:'Precast Deutschland" "['zul.sel.Option','" "'"
//         [
//           "dtid", dtid

//           "cmd_0", "onChange"
//           "uuid_0", "logonid"
//           "data_0", sprintf """{"value":"%i","start":0}""" userId

//           "cmd_1", "onChange"
//           "uuid_1", "password"
//           "data_1", sprintf """{"value":"%s","start":0}""" password
//           "cmd_2", "onSelect"
//           "uuid_2", "clientno_mask"
//           "data_2", sprintf """{"items":["%s"],"reference":"%s"}""" idOfPrecastAt idOfPrecastAt
//           "cmd_3", "onClick"
//           "uuid_3" , "ok"
//           "data_3", """{"pageX":788.7099609375,"pageY":543.5499877929687,"which":1,"x":68.7099609375,"y":7.54998779296875}"""
//         ]
//         |> dict |> (fun x -> new FormUrlEncodedContent(x))
//     let! r = client.PostAsync(sprintf "%s/SES/zkau;jsessionid=%s" baseAddress sessionId, formContent)
//     if not r.IsSuccessStatusCode then
//         failwith "Login failed"
        
//     let! homePageBody = r.Content.ReadAsStringAsync()
// //    printfn "BODY:\n\n%s\n\n==========================================================" homePageBody

//     if not (homePageBody.Contains """Zeitbuchungen einsehen und Zeiten beantragen""") then
//         failwith "Could not detect valid page after login"
    

//     // get page with saldo
//     let pageId = findBetweenAfter homePageBody "['zul.custom.TabEMS','zemstime" "uuid(" ")"
//     use formContent =
//         [
//           "dtid", dtid

//           "cmd_0", "onOpen"
//           "uuid_0", pageId
//           "data_0", """{"open":true,"reference":"startseite"}"""

//           "cmd_1", "onOpen"
//           "uuid_1", pageId
//           "data_1", """{"open":false}"""

//           "cmd_2", "onOpen"
//           "uuid_2", pageId
//           "data_2", """{"open":true,"reference":"startseite"}"""

//           "cmd_3", "onOpen"
//           "uuid_3" , pageId
//           "data_3", """{"open":false}"""
          
//           "cmd_4", "onSelect"
//           "uuid_4" , "zemstime"
//           "data_4", """{"items":["zemstime"],"reference":"zemstime"}"""
          
//           "cmd_5", "onClick"
//           "uuid_5" , "zemstime"
//           "data_5", """{"pageX":271.7699890136719,"pageY":97.57999420166015,"which":1,"x":70.76998901367187,"y":15.579994201660156}"""
//         ]
//         |> dict |> (fun x -> new FormUrlEncodedContent(x))
//     let! r = client.PostAsync(sprintf "%s/SES/zkau;jsessionid=%s" baseAddress sessionId, formContent)
//     if not r.IsSuccessStatusCode then
//         failwith "Login failed"

//     let! saldoPageBody = r.Content.ReadAsStringAsync()

//     let saldo = findBetweenAfter saldoPageBody "value:'Aktueller Tagessaldo'" "value:'" "'"

// //    printfn "Saldo: %s" saldo


//     if modeOpt.IsSome then
//         // Now make a Buchung

//         let mode = modeOpt.Value

//         let buchungsId = findBuchungsID mode saldoPageBody

//         use formContent =
//             [
//               "dtid", dtid

//               "cmd_0", "onSelect"
//               "uuid_0", "req_code"
//               "data_0", sprintf """{"items":["%s"],"reference":"%s"}""" buchungsId buchungsId

//               "cmd_1", "onClick"
//               "uuid_1", "ok"
//               "data_1", """{"pageX":90,"pageY":522,"which":1,"x":78,"y":7}"""
//             ]
//             |> dict |> (fun x -> new FormUrlEncodedContent(x))
//         let! r = client.PostAsync(sprintf "%s/SES/zkau;jsessionid=%s" baseAddress sessionId, formContent)
//         if not r.IsSuccessStatusCode then
//             failwith "Login failed"
        
//         let! body = r.Content.ReadAsStringAsync()
//         if not (body.Contains "Verbuchte Zeit" && body.Contains (match mode with Login -> "Kommen" | Logout -> "Gehen")) then
//             failwith "Could not detect valid page after buchung"


//     // now logout again

//     use formContent =
//         [
//             "dtid", dtid

//             "cmd_0", "onClick"
//             "uuid_0", "logout_btn"
//             "data_0", """{"pageX":1848,"pageY":60,"which":1,"x":76,"y":4}"""
//         ]
//         |> dict |> (fun x -> new FormUrlEncodedContent(x))
//     let! r = client.PostAsync(sprintf "%s/SES/zkau;jsessionid=%s" baseAddress sessionId, formContent)
//     if not r.IsSuccessStatusCode then
//         failwith "Logout failed"

//     return saldo
// }


