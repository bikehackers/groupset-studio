module GroupSetStudio.Cassette

open Fable.React
open FSharpx
open BikeHackers.Components

let cassette (cassette : Cassette) =
  let sprocketString =
    cassette.Sprockets
    |> Seq.map (sprintf "%it")
    |> String.concat ", "

  div
    []
    [
      h4 [] [ str cassette.ManufacturerProductCode ]
      dl
        []
        [
          dt [] [ str "Manufacturer" ]
          dd [] [ str cassette.ManufacturerCode ]
          dt [] [ str <| sprintf "Sprockets (%i)" (Seq.length cassette.Sprockets) ]
          dd [] [ str sprocketString ]
          dt [] [ str "Sprocket Pitch" ]
          dd [] [ str <| sprintf "%gmm" cassette.SprocketPitch ]
          dt [] [ str "Interface" ]
          dd [] [ str cassette.Interface ]
        ]
    ]
