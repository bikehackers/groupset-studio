module GroupSetStudio

open Fable
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Browser
open Browser.Types

type Side =
  | Left
  | Right

type Handedness =
  | Specific of Side
  | Ambi

type RearDerailleur =
  {
    Manufacturer : string
    ProductCode : string
    ActuationRatio : int * int
    Speed : int
    Weight : int
    LowestGear : int
    Capacity : int
    Clutched : bool
  }

type Shifter =
  {
    Manufacturer : string
    ProductCode : string
    Speed : int
    CablePull : float
    Side : Handedness
  }

type Cassette =
  {
    Manufacturer : string
    ProductCode : string
    Cogs : int list
    SprocketPitch : float
    Interface : string
  }

type GroupSet =
  {
    LeftShifter : Shifter
    RightShifter : Shifter
    RearDerailleur : RearDerailleur
    Cassette : Cassette
  }

type CompatabilityIssue =
  | ShifterHandedness of Side * Shifter
  | SprocketPitchMismatch of float * float
  | RearSpeedMismatch of int * int

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

    if groupSet.RightShifter.Speed <> List.length groupSet.Cassette.Cogs
    then
      yield RearSpeedMismatch (groupSet.RightShifter.Speed, List.length groupSet.Cassette.Cogs)
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
      CablePull = 2.7
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
      LowestGear = 42
      Clutched = true
      Weight = 267
      Speed = 11
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-M771"
      Speed = 9
      LowestGear = 34
      Weight = 239
      ActuationRatio = 17, 10
      Capacity = 45
      Clutched = false
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-4700-SS"
      Speed = 10
      LowestGear = 28
      Weight = 275
      ActuationRatio = 79, 54 // Computed
      Capacity = 33
      Clutched = false
    }
    {
      Manufacturer = "shimano"
      ProductCode = "RD-4700-GS"
      Speed = 10
      LowestGear = 34
      Weight = 275
      ActuationRatio = 79, 54 // Computed
      Capacity = 39
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
      Cogs = [ 11; 13; 15; 17; 19; 21; 24; 28; 32; 36 ]
      SprocketPitch = 3.95
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
    RearDerailleur = rearDerailleurs |> Seq.find (fun x -> x.ProductCode = "RD-M771")
    Cassette = cassettes |> Seq.find (fun x -> x.ProductCode = "CS-HG50-10")
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
      [ str (sprintf "Your cassette sprocket pitch does not match your rear derailleur action (%f and %f)" x y) ]

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
            h1 [] [ str "GroupSet Studio" ]
          ]

        h2 [] [ str "Components" ]

        h3 [] [ str "Left Shifter" ]
        select
          []
          (
            shifters
            |> Seq.map (fun shifter ->
              option
                [ Name shifter.ProductCode ]
                [ str (shifter.Manufacturer + " " + shifter.ProductCode) ]
            )
            |> Seq.toList
          )

        h3 [] [ str "Right Shifter" ]
        select
          []
          (
            shifters
            |> Seq.map (fun shifter ->
              option
                [ Name shifter.ProductCode ]
                [ str (shifter.Manufacturer + " " + shifter.ProductCode) ]
            )
            |> Seq.toList
          )

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

        h3 [] [ str "Cassettes" ]
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
