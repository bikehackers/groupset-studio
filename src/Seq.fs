module GroupSetStudio.Seq

let tryMax xs =
  if Seq.isEmpty xs
  then
    None
  else
    Seq.max xs |> Some

let tryMin xs =
  if Seq.isEmpty xs
  then
    None
  else
    Seq.min xs |> Some
