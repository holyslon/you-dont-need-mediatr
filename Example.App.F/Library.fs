namespace Example.App.F

module Calculator =

  let fibonacciFormula state =
    Some(fst state + snd state, (snd state, fst state + snd state))

  let fibonacciSeq = (0, 1) |> Seq.unfold fibonacciFormula

  let factorialFormula ((number, value): int * int) =
    Some(value, (number + 1, value * (number + 1)))

  let factorialSeq = (0, 1) |> Seq.unfold factorialFormula

  type NotPositiveNumberError = string
  type ToBigNumberError = string

  type NumberValidationError =
    | NotPositiveNumberError
    | ToBigNumberError

  type Rule<'value, 'error> = 'value -> Result<'value, 'error>

  let numGreaterThanZeroRule: Rule<int, NotPositiveNumberError> =
    fun num ->
      if num > 0
      then Ok num
      else Error $"Number %i{num} must be greater than 0"

  let numLessThanHundredRule: Rule<int, ToBigNumberError> =
    fun num ->
      if num < 100
      then Ok num
      else Error $"Number %i{num} must be less than 100"

  let validate num =
    numGreaterThanZeroRule num
    |> Result.bind numLessThanHundredRule

  let Calculate num =
    Ok(fibonacciSeq |> Seq.take num |> Seq.last, factorialSeq |> Seq.take num |> Seq.last)

  let Handle num = validate num |> Result.bind Calculate
