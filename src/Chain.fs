module GroupSetStudio.Chain

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types
open FSharpx

let chain (chain : Chain) =
  div
    []
    [
      h4 [] [ str chain.ProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str chain.Manufacturer ]
          dt [] [ str "Speed" ]
          dd [] [ str <| string chain.Speed ]
          dt [] [ str "Weight" ]
          dd [] [ str <| sprintf "%ig" chain.Weight ]
        ]
    ]
