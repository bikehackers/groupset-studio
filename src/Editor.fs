module GroupSetStudio.Editor

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open GroupSetStudio
open GroupSetStudio.Shifter
open GroupSetStudio.RearDerailleur
open GroupSetStudio.Cassette
open GroupSetStudio.Chain
open BikeHackers.Components

type GroupSet =
  {
    LeftShifter : IntegratedShifter
    RightShifter : IntegratedShifter
    RearDerailleur : RearDerailleur
    Cassette : Cassette
    Chain : Chain
  }

type CompatabilityIssue =
  | ShifterHandedness of Side * IntegratedShifter
  | SprocketPitchMismatch of float * float
  | RearSpeedMismatch of int * int
  | ChainCassetteSpeedMismatch of int * int
  | RearCassetteTooLarge of int * int

let ratioToFloat (n, d) =
  float n / float d

let findIssues (groupSet : GroupSet) : CompatabilityIssue list =
  seq {
    if groupSet.LeftShifter.Hand = Handedness.Specific Right
    then
      yield ShifterHandedness (Left, groupSet.LeftShifter)

    if groupSet.RightShifter.Hand = Handedness.Specific Left
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

type State =
  {
    GroupSet : GroupSet
  }

type Props =
  {
    DefaultGroupset : GroupSet
    RearDerailleurs : RearDerailleur list
    Chains : Chain list
    Shifters : IntegratedShifter list
    Cassettes : Cassette list
  }

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

type Editor (initProps) =
  inherit Component<Props, State> (initProps) with

  do
    base.setInitState
      {
        GroupSet = initProps.DefaultGroupset
      }

  override this.render () =
    let groupSet = this.state.GroupSet

    let chains = this.props.Chains
    let shifters = this.props.Shifters
    let cassettes = this.props.Cassettes
    let rearDerailleurs = this.props.RearDerailleurs

    let issues = findIssues groupSet

    fragment
      []
      [
        header
          []
          [
            h1 [] [ str "Groupset Studio" ]
          ]

        h2 [] [ str "Limitations" ]
        ul
          []
          [
            li [] [ str "Only the rear shifting is currently checked. " ]
            li [] [ str "Electronic shifting is not supported. " ]
            li [] [ str "Brake systems are not checked. " ]
          ]

        h2 [] [ str "Components" ]

        h3 [] [ str "Left Shifter" ]
        select
          [
            Value (Option.defaultValue "" groupSet.LeftShifter.ManufacturerProductCode)
            OnChange
              (fun e ->
                let productCode : string = !!e.target?value

                let maybeSelected =
                  shifters
                  |> Seq.tryFind (fun x ->
                    Option.defaultValue "" x.ManufacturerProductCode = productCode
                  )

                match maybeSelected with
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
                [ Value shifter.ManufacturerProductCode ]
                [ str (shifter.ManufacturerCode + " " + Option.defaultValue "" shifter.ManufacturerProductCode) ]
            )
            |> Seq.toList
          )
        shifter groupSet.LeftShifter

        h3 [] [ str "Right Shifter" ]
        select
          [
            Value groupSet.RightShifter.ManufacturerProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match shifters |> Seq.tryFind (fun x -> x.ManufacturerProductCode = productCode) with
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
                [ Value shifter.ManufacturerProductCode ]
                [ str (shifter.ManufacturerCode + " " + Option.defaultValue "" shifter.ManufacturerProductCode) ]
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
            Value groupSet.Cassette.ManufacturerProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match cassettes |> Seq.tryFind (fun x -> x.ManufacturerProductCode = productCode) with
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
                [ Value c.ManufacturerProductCode ]
                [ str (c.ManufacturerCode + " " + c.ManufacturerProductCode) ]
            )
            |> Seq.toList
          )
        cassette groupSet.Cassette

        h3 [] [ str "Chain" ]
        select
          [
            Value groupSet.Chain.ManufacturerProductCode
            OnChange
              (fun e ->
                let productCode = !!e.target?value

                match chains |> Seq.tryFind (fun x -> x.ManufacturerProductCode = productCode) with
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
                [ Value c.ManufacturerProductCode ]
                [ str (c.ManufacturerCode + " " + c.ManufacturerProductCode) ]
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

let editor initProps =
  ofType<Editor, Props, State> initProps []
