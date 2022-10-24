using Example.App.Services.Calculation;

namespace Example.App.Native;

public record CalculateRequest(int Target);

public class CalculationUnit
{
    private readonly FactorialService _factorialService;
    private readonly FibonacciService _fibonacciService;

    public CalculationUnit(FactorialService factorialService, FibonacciService fibonacciService)
    {
        _factorialService = factorialService;
        _fibonacciService = fibonacciService;
    }
    
    public Task<(int Fibonachi, int Factorial)> DoCalculate(CalculateRequest request)
    {
        return Task.FromResult((_factorialService.GetFactorial(request.Target), _fibonacciService.GetNumber(request.Target)));
    }
}