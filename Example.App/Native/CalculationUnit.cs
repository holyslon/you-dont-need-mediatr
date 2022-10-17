using Example.App.Services.Calculation;

namespace Example.App.Native;

public class CalculationUnit
{
    private readonly FactorialService _factorialService;
    private readonly FibonacciService _fibonacciService;

    public CalculationUnit(FactorialService factorialService, FibonacciService fibonacciService)
    {
        _factorialService = factorialService;
        _fibonacciService = fibonacciService;
    }
    
    public Task<(int Fibonachi, int Factorial)> DoCalulate(int target)
    {
        using var _ = _metrics.StartWork(nameof(CalculationUnit));
        _validator.Validate(target);
        _cache.GetOrAdd() 
        return Task.FromResult((_factorialService.GetFactorial(target), _fibonacciService.GetNumber(target)));
    }
}