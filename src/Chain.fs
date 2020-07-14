module GroupSetStudio.Chain

open Fable.React
open FSharpx
open BikeHackers.Components

let chain (chain : Chain) =
  div
    []
    [
      h4 [] [ str chain.ManufacturerProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str chain.ManufacturerCode ]
          dt [] [ str "Speed" ]
          dd [] [ str <| string chain.Speed ]
          dt [] [ str "Weight" ]
          dd
            []
            [
              chain.Weight
              |> Option.map (sprintf "%ig")
              |> Option.defaultValue "-"
              |> str
            ]
        ]
    ]
