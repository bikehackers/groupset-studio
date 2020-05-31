module GroupSetStudio.Shifter

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types

let shifter (shifter : Shifter) =
  div
    []
    [
      h4 [] [ str shifter.ProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str shifter.Manufacturer ]
          dt [] [ str "Speeds" ]
          dd [] [ str <| string shifter.Speed ]
          dt [] [ str "Cable Pull" ]
          dd [] [ str <| sprintf "%gmm" shifter.CablePull ]
          dt [] [ str "Handedness" ]
          dd
            []
            [
              match shifter.Side with
              | Specific s -> string s
              | Ambi -> "Ambidextrous"
              |> str
            ]
        ]
    ]
