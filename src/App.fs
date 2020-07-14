module GroupSetStudio.App

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Browser
open GroupSetStudio
open GroupSetStudio.Editor
open BikeHackers.Components

type Props = obj

type Status =
  | Loading
  | Ready of Api.Data
  | Error of Exception

type State =
  {
    Status : Status
  }

type App (initProps) =
  inherit Component<Props, State> (initProps) with

  do
    base.setInitState
      {
        Status = Loading
      }

  override this.componentDidMount () =
    async {
      try
        let! data = Api.fetchData

        this.setState (fun s p -> { s with Status = Ready data })
      with exn ->
        this.setState (fun s p -> { s with Status = Error exn })
    }
    |> Async.StartAsPromise
    |> ignore

  override this.render () =
    match this.state.Status with
    | Loading -> div [] [ str "Loading... " ]
    | Ready data ->
      editor
        {
          DefaultGroupset =
            {
              LeftShifter =
                data.Shifters
                |> List.find (fun x -> x.ManufacturerProductCode = Some "ST-RX810-L")
              RightShifter =
                data.Shifters
                |> List.find (fun x -> x.ManufacturerProductCode = Some "ST-RX810-R")
              RearDerailleur =
                data.RearDerailleurs
                |> List.find (fun x -> x.ProductCode = "RD-RX812")
              Cassette =
                data.Cassettes
                |> List.find (fun x -> x.ManufacturerProductCode = "CS-M8000-BS")
              Chain =
                data.Chains
                |> List.find (fun x -> x.ManufacturerProductCode = "CN-HG701")
            }
          RearDerailleurs = data.RearDerailleurs
          Chains = data.Chains
          Shifters = data.Shifters
          Cassettes = data.Cassettes
        }
    | Error exn -> div [] [ str (string exn) ]

let app initProps =
  ofType<App, Props, State> initProps []

ReactDom.render
  (
    app createEmpty,
    document.getElementById "root"
  )
