module GroupSetStudio.RearDerailleur

open Fable.React
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
              match rd.LargestSprocketMinTeeth with
              | Some largestSprocketMinTeeth ->
                if largestSprocketMinTeeth = rd.LargestSprocketMaxTeeth
                then
                  str <| sprintf "%it" rd.LargestSprocketMaxTeeth
                else
                  str <| sprintf "%it-%it" largestSprocketMinTeeth rd.LargestSprocketMaxTeeth
              | None ->
                  str <| sprintf "%it" rd.LargestSprocketMaxTeeth
            ]
          dt [] [ str "Smallest Sprocket" ]
          dd
            []
            [
              match rd.SmallestSprocketMinTeeth, rd.SmallestSprocketMaxTeeth with
              | Some smallestSprocketMinTeeth, Some smallestSprocketMaxTeeth ->
                if smallestSprocketMinTeeth = smallestSprocketMaxTeeth
                then
                  str <| sprintf "%it" smallestSprocketMaxTeeth
                else
                  str <| sprintf "%it-%it" smallestSprocketMinTeeth smallestSprocketMaxTeeth
              | _ ->
                str "-"
            ]
          dt [] [ str "Capacity" ]
          dd
            []
            [
              match rd.Capacity with
              | Some capacity ->
                str <| sprintf "%it" capacity
              | None ->
                str "-"
            ]
        ]
    ]
