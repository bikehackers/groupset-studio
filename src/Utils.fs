namespace GroupSetStudio

module Result =

  let get =
    function
    | Ok x -> x
    | Error e -> failwithf "Expected Ok but got %A" e
