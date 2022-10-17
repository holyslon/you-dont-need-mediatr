namespace Example.App.MediatR.Calculation;

public class CalculationResponse
{
    public CalculationResponse(int factorial, int fibonacciNumber)
    {
        Factorial = factorial;
        FibonacciNumber = fibonacciNumber;
    }

    public int Factorial { get; }
    public int FibonacciNumber { get; }
}