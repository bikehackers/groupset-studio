module GroupSetStudio.App

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types
open GroupSetStudio
open GroupSetStudio.Shifter
open GroupSetStudio.RearDerailleur
open GroupSetStudio.Cassette
open GroupSetStudio.Chain

type GroupSet =
  {
    LeftShifter : Shifter
    RightShifter : Shifter
    RearDerailleur : RearDerailleur
    Cassette : Cassette
    Chain : Chain
  }

type CompatabilityIssue =
  | ShifterHandedness of Side * Shifter
  | SprocketPitchMismatch of float * float
  | RearSpeedMismatch of int * int
  | ChainCassetteSpeedMismatch of int * int
  | RearCassetteTooLarge of int * int

let ratioToFloat (n, d) =
  float n / float d

let findIssues (groupSet : GroupSet) : CompatabilityIssue list =
  seq {
    if groupSet.LeftShifter.Side = Handedness.Specific Right
    then
      yield ShifterHandedness (Left, groupSet.LeftShifter)

    if groupSet.RightShifter.Side = Handedness.Specific Left
    then
      yield ShifterHandedness (Right, groupSet.RightShifter)

    let expectedSprocketPitch =
      groupSet.RightShifter.CablePull * (ratioToFloat groupSet.RearDerailleur.ActuationRatio)

    // (3.95 - 3.91) / 3.91 * 100

    if abs (expectedSprocketPitch - groupSet.Cassette.SprocketPitch) > 0.1
    then
      yield SprocketPitchMismatch (expectedSprocketPitch, groupSet.Cassette.SprocketPitch)

    if groupSet.RightShifter.Speed <> List.length groupSet.Cassette.Sprockets
    then
      yield RearSpeedMismatch (groupSet.RightShifter.Speed, List.length groupSet.Cassette.Sprockets)

    if groupSet.Chain.Speed <> List.length groupSet.Cassette.Sprockets
    then
      yield ChainCassetteSpeedMismatch (groupSet.Chain.Speed, List.length groupSet.Cassette.Sprockets)

    match Seq.tryMax groupSet.Cassette.Sprockets with
    | Some t ->
      if t > groupSet.RearDerailleur.LargestSprocketMaxTeeth
      then
        yield RearCassetteTooLarge (t, groupSet.RearDerailleur.LargestSprocketMaxTeeth)
    | None -> ()
  }
  |> Seq.toList

let shifters =
  [
    {
      Manufacturer = "shimano"
      ProductCode = "ST-5700-L"
      Speed = 2
      Side = Specific Left
      CablePull = 2.3
    }
    {
      Manufacturer = "shimano"
      ProductCode = "ST-5700-R"
      Speed = 10
      Side = Specific Right
      CablePull = 2.3
    }
    {
      Manufacturer = "shimano"
      ProductCode = "ST-4700-R"
      Speed = 10
      Side = Specific Right
      CablePull = 3.95 / 1.4
    }
    {
      Manufacturer = "sram"
      ProductCode = "SB-APX-B1"
      Speed = 11
      Side = Specific Right
      CablePull = 3.1
    }
  ]
  |> List.sortBy (fun x -> x.Manufacturer, x.ProductCode)

let rearDerailleurs =
  [
    {
      Manufacturer = "shimano"
      ProductCode = "RD-RX812"
      ActuationRatio = 27, 10
      Capacity = 31
      LargestSprocketMaxTeeth = 42
      LargestSprocketMinTeeth = 40
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 11
      Clutched = true
      Weight = 267
      Speed = 11
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-M771-SGS"
      Speed = 9
      LargestSprocketMaxTeeth = 34
      LargestSprocketMinTeeth = 32
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 11
      Weight = 239
      ActuationRatio = 17, 10
      Capacity = 45
      Clutched = false
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-4700-SS"
      Speed = 10
      LargestSprocketMaxTeeth = 28
      LargestSprocketMinTeeth = 25
      SmallestSprocketMaxTeeth = 14
      SmallestSprocketMinTeeth = 11
      Weight = 275
      ActuationRatio = 14, 10
      Capacity = 33
      Clutched = false
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-4700-GS"
      Speed = 10
      LargestSprocketMaxTeeth = 34
      LargestSprocketMinTeeth = 28
      SmallestSprocketMaxTeeth = 12
      SmallestSprocketMinTeeth = 11
      Weight = 275
      ActuationRatio = 14, 10
      Capacity = 41
      Clutched = false
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-M592-SGS"
      Speed = 9
      LargestSprocketMaxTeeth = 36
      LargestSprocketMinTeeth = 32
      SmallestSprocketMaxTeeth = 12
      SmallestSprocketMinTeeth = 11
      Weight = 286
      ActuationRatio = 17, 10
      Capacity = 45
      Clutched = false
    }
    {
      Manufacturer = "sram"
      ProductCode = "RD-APX-1-A1"
      Speed = 11
      LargestSprocketMaxTeeth = 42
      LargestSprocketMinTeeth = 0
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 0
      Weight = 312
      ActuationRatio = 13, 10
      Capacity = 0
      Clutched = true
    }
    {
      Manufacturer = "sram"
      ProductCode = "RD-X5-M"
      Speed = 9
      LargestSprocketMaxTeeth = 36
      LargestSprocketMinTeeth = 0
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 0
      Weight = 295
      ActuationRatio = 1, 1
      Capacity = 0
      Clutched = true
    }
    {
      Manufacturer = "microshift"
      ProductCode = "RD-R55S"
      Speed = 10
      LargestSprocketMaxTeeth = 32
      LargestSprocketMinTeeth = 25
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 11
      Weight = 195
      ActuationRatio = 27, 10
      Capacity = 35
      Clutched = false
    }
    {
      Manufacturer = "sram"
      ProductCode = "RD-NX-1-A1"
      Speed = 11
      Weight = 322
      ActuationRatio = 348, 100
      LargestSprocketMaxTeeth = 42
      LargestSprocketMinTeeth = 0
      SmallestSprocketMaxTeeth = 10
      SmallestSprocketMinTeeth = 0
      Capacity = 0
      Clutched = true
    }
    {
      Manufacturer = "campagnolo"
      ProductCode = "rd-chorus-2015-11-short"
      Speed = 11
      Weight = 183
      ActuationRatio = 15, 10
      LargestSprocketMaxTeeth = 29
      LargestSprocketMinTeeth = 0
      SmallestSprocketMaxTeeth = 11
      SmallestSprocketMinTeeth = 0
      Capacity = 34
      Clutched = false
    }
  ]
  |> List.sortBy (fun x -> x.Manufacturer, x.ProductCode)

let cassettes =
  [
    {
      Manufacturer = "shimano"
      ProductCode = "CS-HG50-10"
      Interface = "hyperglide"
      Sprockets = [ 11; 13; 15; 17; 19; 21; 24; 28; 32; 36 ]
      SprocketPitch = 3.95
    }
    {
      Manufacturer = "shimano"
      ProductCode = "CS-M8000-BS"
      Interface = "hyperglide"
      Sprockets = [ 11; 13; 15; 17; 19; 21; 24; 27; 31; 35; 40 ]
      SprocketPitch = 3.95
    }
    {
      Manufacturer = "shimano"
      ProductCode = "CS-HG81-10-BK"
      Interface = "hyperglide"
      Sprockets = [ 11; 13; 15; 17; 19; 21; 24; 28; 32; 36 ]
      SprocketPitch = 3.95
    }
  ]
  |> List.sortBy (fun x -> x.Manufacturer, x.ProductCode)

let chains =
  [
    {
      Manufacturer = "kmc"
      ProductCode = "BX09SLT14"
      Speed = 9
      Weight = 272
    }
    {
      Manufacturer = "kmc"
      ProductCode = "BX10ELT14"
      Speed = 10
      Weight = 262
    }
    {
      Manufacturer = "shimano"
      ProductCode = "CH-HG701-11"
      Speed = 11
      Weight = 257
    }
    {
      Manufacturer = "shimano"
      ProductCode = "CN-HG54"
      Speed = 10
      Weight = 273
    }
  ]
  |> List.sortBy (fun x -> x.Manufacturer, x.ProductCode)

type State =
  {
    GroupSet : GroupSet
  }

let defaultGroupSet =
  {
    LeftShifter = shifters |> Seq.find (fun x -> x.ProductCode = "ST-5700-L")
    RightShifter = shifters |> Seq.find (fun x -> x.ProductCode = "ST-5700-R")
    RearDerailleur = rearDerailleurs |> Seq.find (fun x -> x.ProductCode = "RD-M592-SGS")
    Cassette = cassettes |> Seq.find (fun x -> x.ProductCode = "CS-HG50-10")
    Chain = chains |> Seq.find (fun x -> x.ProductCode = "CN-HG54")
  }

type Props = obj

let renderIssue issue =
  match issue with
  | ShifterHandedness (side, shifter) ->
    div
      []
      [ str (sprintf "Your %A shifter is not designed to be on that side. " side) ]
  | RearSpeedMismatch (x, y) ->
    div
      []
      [ str (sprintf "Your cassette and shifter have mismatched speeds (%i and %i)" x y) ]
  | SprocketPitchMismatch (x, y) ->
    div
      []
      [ str (sprintf "Your rear derailleur action (%gmm) does not match your cassette sprocket pitch (%gmm)" x y) ]
  | ChainCassetteSpeedMismatch (x, y) ->
    div
      []
      [ str (sprintf "Your chain and cassette have mismatched speeds (%i and %i)" x y) ]
  | RearCassetteTooLarge (x, y) ->
    div
      []
      [ str (sprintf "Your rear cassette (%it) is too large for your rear derailleur (%it)" x y) ]

type App (initProps) =
  inherit Component<Props, State> (initProps) with

  do
    base.setInitState
      {
        GroupSet = defaultGroupSet
      }

  override this.render () =
    let groupSet = this.state.GroupSet

    let issues = findIssues groupSet

    fragment
      []
      [
        header
          []
          [
            h1 [] [ str "Group Set Studio" ]
          ]

        h2 [] [ str "Components" ]

        h3 [] [ str "Left Shifter" ]
        select
          [
            Value groupSet.LeftShifter.ProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match shifters |> Seq.tryFind (fun x -> x.ProductCode = productCode) with
                | Some sh ->
                  this.setState
                    (fun s p ->
                      {
                        s with
                          GroupSet =
                            {
                              s.GroupSet with
                                LeftShifter = sh
                            }
                      })
                | None -> ()
              )
          ]
          (
            shifters
            |> Seq.map (fun shifter ->
              option
                [ Value shifter.ProductCode ]
                [ str (shifter.Manufacturer + " " + shifter.ProductCode) ]
            )
            |> Seq.toList
          )
        shifter groupSet.LeftShifter

        h3 [] [ str "Right Shifter" ]
        select
          [
            Value groupSet.RightShifter.ProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match shifters |> Seq.tryFind (fun x -> x.ProductCode = productCode) with
                | Some sh ->
                  this.setState
                    (fun s p ->
                      {
                        s with
                          GroupSet =
                            {
                              s.GroupSet with
                                RightShifter = sh
                            }
                      })
                | None -> ()
              )
          ]
          (
            shifters
            |> Seq.map (fun shifter ->
              option
                [ Value shifter.ProductCode ]
                [ str (shifter.Manufacturer + " " + shifter.ProductCode) ]
            )
            |> Seq.toList
          )
        shifter groupSet.RightShifter

        h3 [] [ str "Rear Derailleur" ]
        select
          [
            Value groupSet.RearDerailleur.ProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match rearDerailleurs |> Seq.tryFind (fun x -> x.ProductCode = productCode) with
                | Some rd ->
                  this.setState
                    (fun s p ->
                      {
                        s with
                          GroupSet =
                            {
                              s.GroupSet with
                                RearDerailleur = rd
                            }
                      })
                | None -> ()
              )
          ]
          (
            rearDerailleurs
            |> Seq.map (fun rd ->
              option
                [ Value rd.ProductCode ]
                [ str (rd.Manufacturer + " " + rd.ProductCode) ]
            )
            |> Seq.toList
          )
        rearDerailleur groupSet.RearDerailleur

        h3 [] [ str "Cassette" ]
        select
          [
            Value groupSet.Cassette.ProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match cassettes |> Seq.tryFind (fun x -> x.ProductCode = productCode) with
                | Some c ->
                  this.setState
                    (fun s p ->
                      {
                        s with
                          GroupSet =
                            {
                              s.GroupSet with
                                Cassette = c
                            }
                      })
                | None -> ()
              )
          ]
          (
            cassettes
            |> Seq.map (fun c ->
              option
                [ Value c.ProductCode ]
                [ str (c.Manufacturer + " " + c.ProductCode) ]
            )
            |> Seq.toList
          )
        cassette groupSet.Cassette

        h3 [] [ str "Chain" ]
        select
          [
            Value groupSet.Chain.ProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match chains |> Seq.tryFind (fun x -> x.ProductCode = productCode) with
                | Some c ->
                  this.setState
                    (fun s p ->
                      {
                        s with
                          GroupSet =
                            {
                              s.GroupSet with
                                Chain = c
                            }
                      })
                | None -> ()
              )
          ]
          (
            chains
            |> Seq.map (fun c ->
              option
                [ Value c.ProductCode ]
                [ str (c.Manufacturer + " " + c.ProductCode) ]
            )
            |> Seq.toList
          )
        chain groupSet.Chain

        h2 [] [ str "Issues" ]
        (
          if Seq.isEmpty issues
          then
            p [] [ str "No issues were detected! Remember that this is not a guarantee. " ]
          else
            ul
              []
              (
                issues
                |> Seq.map (fun issue ->
                  li
                    []
                    [
                      renderIssue issue
                    ]
                )
                |> Seq.toList
              )
        )
      ]

let app initProps =
  ofType<App, Props, State> initProps []

ReactDom.render
  (
    app createEmpty,
    document.getElementById "root"
  )
