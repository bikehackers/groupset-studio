module GroupSetStudio.Shifter

open Fable.React
open BikeHackers.Components

let shifter (shifter : IntegratedShifter) =
  div
    []
    [
      h4 [] [ str (Option.defaultValue "-" shifter.ManufacturerProductCode) ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str shifter.ManufacturerCode ]
          dt [] [ str "Speeds" ]
          dd [] [ str <| string shifter.Speed ]
          dt [] [ str "Cable Pull" ]
          dd [] [ str <| sprintf "%gmm" shifter.CablePull ]
          dt [] [ str "Handedness" ]
          dd
            []
            [
              match shifter.Hand with
              | Specific s -> string s
              | Ambi -> "Ambidextrous"
              |> str
            ]
        ]
    ]
