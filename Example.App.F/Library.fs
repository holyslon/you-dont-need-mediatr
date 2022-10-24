namespace Example.App.F

module Calculator =
        
    let fibonacciFormula state = Some(fst state + snd state, (snd state, fst state + snd state))
        
    let fibonacciSeq = (0,1) |> Seq.unfold fibonacciFormula
    
    let factorialFormula ((number, value): int * int) = Some(value, (number + 1, value * (number + 1)))
    
    let factorialSeq = (0,1) |> Seq.unfold factorialFormula
    
    let Calculate num = 
        Some(fibonacciSeq |> Seq.take num |> Seq.last, factorialSeq |> Seq.take num |> Seq.last)
        