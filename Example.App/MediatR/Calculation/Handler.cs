using Example.App.Services.Calculation;
using MediatR;

namespace Example.App.MediatR.Calculation;

public class Handler : IRequestHandler<CalculationRequest, CalculationResponse>
{
    private readonly FactorialService _factorialService;
    private readonly FibonacciService _fibonacciService;

    public Handler(FactorialService factorialService, FibonacciService fibonacciService)
    {
        _factorialService = factorialService;
        _fibonacciService = fibonacciService;
    }

    public Task<CalculationResponse> Handle(CalculationRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CalculationResponse(
            _factorialService.GetFactorial(request.Target),
            _fibonacciService.GetNumber(request.Target)
        ));
    }
}