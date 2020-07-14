#load @".paket/load/netcoreapp3.1/Newtonsoft.Json.fsx"
#load @".paket/load/netcoreapp3.1/main.group.fsx"

#load @"./external/components-db/components/Types.fs"
#load @"./external/components-db/components/Thoth.fs"

open System
open System.IO
open Thoth.Json.Net
open BikeHackers.Components
open BikeHackers.Components.Thoth
open DotNet.Globbing

module Result =

  let get =
    function
    | Ok o -> o
    | Error e -> failwith (sprintf "Expected Result.Ok but got %A" e)

let rec private filesUnderPath (basePath : string) =
  seq {
    if Directory.Exists basePath
    then
      for f in Directory.GetFiles (basePath) do
        yield f

      for d in Directory.GetDirectories (basePath) do
        yield! filesUnderPath d
  }

async {
  let glob = Glob.Parse "**/data/rear-derailleurs/**/*.json"

  let! rearDerailleurs =
    filesUnderPath "./external/components-db/data/rear-derailleurs"
    |> Seq.filter glob.IsMatch
    |> Seq.map (fun filePath ->
      async {
        let! content =
          File.ReadAllTextAsync filePath
          |> Async.AwaitTask

        return
          Decode.fromString Decode.rearDerailleur content
          |> Result.get
      })
    |> Async.Parallel

  let glob = Glob.Parse "**/data/cassettes/**/*.json"

  let! cassettes =
    filesUnderPath "./external/components-db/data/cassettes"
    |> Seq.filter glob.IsMatch
    |> Seq.map (fun filePath ->
      async {
        let! content =
          File.ReadAllTextAsync filePath
          |> Async.AwaitTask

        return
          Decode.fromString Decode.cassette content
          |> Result.get
      })
    |> Async.Parallel

  let glob = Glob.Parse "**/data/chains/**/*.json"

  let! chains =
    filesUnderPath "./external/components-db/data/chains"
    |> Seq.filter glob.IsMatch
    |> Seq.map (fun filePath ->
      async {
        let! content =
          File.ReadAllTextAsync filePath
          |> Async.AwaitTask

        return
          Decode.fromString Decode.chain content
          |> Result.get
      })
    |> Async.Parallel

  let glob = Glob.Parse "**/data/integrated-shifters/**/*.json"

  let! integratedShifters =
    filesUnderPath "./external/components-db/data/integrated-shifters"
    |> Seq.filter glob.IsMatch
    |> Seq.map (fun filePath ->
      async {
        let! content =
          File.ReadAllTextAsync filePath
          |> Async.AwaitTask

        return
          Decode.fromString Decode.integratedShifter content
          |> Result.get
      })
    |> Async.Parallel

  let content =
    [
      (
        "rearDerailleurs",
        Encode.list (rearDerailleurs |> Seq.map Encode.rearDerailleur |> Seq.toList)
      )
      (
        "cassettes",
        Encode.list (cassettes |> Seq.map Encode.cassette |> Seq.toList)
      )
      (
        "chains",
        Encode.list (chains |> Seq.map Encode.chain |> Seq.toList)
      )
      (
        "integratedShifters",
        Encode.list (integratedShifters |> Seq.map Encode.integratedShifter |> Seq.toList)
      )
    ]
    |> Encode.object
    |> Encode.toString 2

  do!
    File.WriteAllTextAsync ("./public/blob.json", content)
    |> Async.AwaitTask

  ()
}
|> Async.RunSynchronously
