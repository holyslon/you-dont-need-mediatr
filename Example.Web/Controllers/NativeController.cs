using Example.App.Logging;
using Example.App.Metrics;
using Example.App.Native;
using Example.App.Tracing;
using Example.App.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;
 
[ApiController]
[Route("native")]
public class NativeController : ControllerBase
{
    private readonly CalculationUnit _unit;
    private readonly IScope _scope;
    public NativeController(CalculationUnit unit, IScope scope)
    {
        _unit = unit;
        _scope = scope;
    }

    public record CalculateInput(int? target);

    public record Response(int factorial, int fibonacciNumber)
    {
        
    }

    private static int ValidateCalculateInput(CalculateInput input)
    {
        if (input.target == null)
        {
            throw new ValidationException(nameof(input.target), "should present");
        }

        var targetValue = input.target.Value;
        return targetValue switch
        {
            < 0 => throw new ValidationException(nameof(input.target), "should be 0 or more"),
            > 19 => throw new ValidationException(nameof(input.target), "should be less than 20"),
            _ => targetValue
        };
    }

    [HttpPost("calculate")]
    public async Task<Response> Calculate([FromBody] CalculateInput input)
    {
        var target = ValidateCalculateInput(input);
        using(_scope.WithScope(input))
        using(Elapsed.WithMeter<MediatRController.CalculateInput>())
        using(Trace.WithTrace<MediatRController.CalculateInput>())
        {
            var (fibonachi, factorial) = await _unit.DoCalculate(target);
            return new Response(fibonachi, factorial);
        }
    }
}