module GroupSetStudio.RearDerailleur

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types

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
          dt [] [ str "Largest Cog" ]
          dd
            []
            [
              if rd.LargestCogMinTeeth = rd.LargestCogMaxTeeth
              then
                str <| sprintf "%it" rd.LargestCogMaxTeeth
              else
                str <| sprintf "%it-%it" rd.LargestCogMinTeeth rd.LargestCogMaxTeeth
            ]
          dt [] [ str "Smallest Cog" ]
          dd
            []
            [
              if rd.SmallestCogMinTeeth = rd.SmallestCogMaxTeeth
              then
                str <| sprintf "%it" rd.SmallestCogMaxTeeth
              else
                str <| sprintf "%it-%it" rd.SmallestCogMinTeeth rd.SmallestCogMaxTeeth
            ]
          dt [] [ str "Capacity" ]
          dd [] [ str <| sprintf "%it" rd.Capacity ]
        ]
    ]
