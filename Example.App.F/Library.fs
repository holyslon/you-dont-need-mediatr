namespace Example.App.F

module Calculator =

  let fibonacciFormula state =
    Some(fst state + snd state, (snd state, fst state + snd state))

  let fibonacciSeq = (0, 1) |> Seq.unfold fibonacciFormula

  let factorialFormula ((number, value): int * int) =
    Some(value, (number + 1, value * (number + 1)))

  let factorialSeq = (0, 1) |> Seq.unfold factorialFormula

  type NumberValidationError =
    | NotPositiveNumberError of string
    | ToBigNumberError of string

  type Rule<'value, 'error> = 'value -> Result<'value, 'error>

  let numGreaterThanZeroRule: Rule<int, NumberValidationError> =
    fun num ->
      if num > 0
      then Ok num
      else Error(NumberValidationError.NotPositiveNumberError $"Number %i{num} must be greater than 0")

  let numLessThanHundredRule: Rule<int, NumberValidationError> =
    fun num ->
      if num < 100
      then Ok num
      else Error(NumberValidationError.ToBigNumberError $"Number %i{num} must be less than 100")

  let validate num =
    numGreaterThanZeroRule num
    |> Result.bind numLessThanHundredRule

  let Calculate num =
    Ok(fibonacciSeq |> Seq.take num |> Seq.last, factorialSeq |> Seq.take num |> Seq.last)

  let (>>=) f1 f2 = f1 |> Result.bind f2
  let kleisly f2 f1 = fun v -> f1 v >>= f2
  let (>=>) f1 f2 = f1 |> kleisly f2

  let validateV2 =
    numGreaterThanZeroRule >=> numLessThanHundredRule

  let rules =
    [ numGreaterThanZeroRule
      numLessThanHundredRule ]

  let validateV3 num = num |> List.reduce (>=>) rules

  let Handle num = validate num |> Result.bind Calculate
