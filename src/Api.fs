module GroupSetStudio.Api

open BikeHackers.Components
open BikeHackers.Components.Thoth
open Fable.SimpleHttp

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type Data =
  {
    Shifters : IntegratedShifter list
    Chains : Chain list
    RearDerailleurs : RearDerailleur list
    Cassettes : Cassette list
  }

module Thoth =

  module Decode =

    let data =
      Decode.object
        (fun get ->
          {
            Shifters = get.Required.Field "integratedShifters" (Decode.list Decode.integratedShifter)
            Cassettes = get.Required.Field "cassettes" (Decode.list Decode.cassette)
            RearDerailleurs = get.Required.Field "rearDerailleurs" (Decode.list Decode.rearDerailleur)
            Chains = get.Required.Field "chains" (Decode.list Decode.chain)
          })

open Thoth

let fetchData : Async<Data> =
  async {
    let! (statusCode, responseText) = Http.get "/blob.json"

    match statusCode with
    | 200 ->
      let data =
        responseText
        |> Decode.fromString Decode.data

      let data = data |> Result.get

      return data
    | _ ->
      return failwithf "Error fetching data %i: %s" statusCode responseText
  }
