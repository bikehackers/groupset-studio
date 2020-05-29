module GroupSetStudio.Cassette

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types
open FSharpx

let cassette (cassette : Cassette) =
  let cogsString =
    cassette.Cogs
    |> Seq.map (sprintf "%it")
    |> String.concat ", "

  div
    []
    [
      h4 [] [ str cassette.ProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str cassette.Manufacturer ]
          dt [] [ str <| sprintf "Cogs (%i)" (Seq.length cassette.Cogs) ]
          dd [] [ str cogsString ]
          dt [] [ str "Sprocket Pitch" ]
          dd [] [ str <| sprintf "%gmm" cassette.SprocketPitch ]
          dt [] [ str "Interface" ]
          dd [] [ str cassette.Interface ]
        ]
    ]
