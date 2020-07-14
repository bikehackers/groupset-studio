module GroupSetStudio.RearDerailleur

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types
open BikeHackers.Components

let rearDerailleur (rd : RearDerailleur) =
  div
    []
    [
      h4 [] [ str rd.ProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str rd.Manufacturer ]
          dt [] [ str "Speeds" ]
          dd [] [ str <| string rd.Speed ]
          dt [] [ str "Actuation Ratio" ]
          dd [] [ str <| sprintf "%i:%i" (fst rd.ActuationRatio) (snd rd.ActuationRatio) ]
          dt [] [ str "Largest Sprocket" ]
          dd
            []
            [
              if rd.LargestSprocketMinTeeth = rd.LargestSprocketMaxTeeth
              then
                str <| sprintf "%it" rd.LargestSprocketMaxTeeth
              else
                str <| sprintf "%it-%it" rd.LargestSprocketMinTeeth rd.LargestSprocketMaxTeeth
            ]
          dt [] [ str "Smallest Sprocket" ]
          dd
            []
            [
              if rd.SmallestSprocketMinTeeth = rd.SmallestSprocketMaxTeeth
              then
                str <| sprintf "%it" rd.SmallestSprocketMaxTeeth
              else
                str <| sprintf "%it-%it" rd.SmallestSprocketMinTeeth rd.SmallestSprocketMaxTeeth
            ]
          dt [] [ str "Capacity" ]
          dd [] [ str <| sprintf "%it" rd.Capacity ]
        ]
    ]
