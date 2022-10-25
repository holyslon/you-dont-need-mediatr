using Example.App.Logging;
using Example.App.MediatR.Calculation;
using Example.App.Metrics;
using Example.App.Native;
using Example.Web.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

[ApiController]
[Route("native")]
public class NativeController : ControllerBase
{
    private readonly CalculationUnit _unit;
    private readonly IScope _scope;
    private readonly IElapsed _elapsed;

    public NativeController(CalculationUnit unit)
    {
        _unit = unit;
    }

    public record CalculateInput(int? target);

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input)
    {
        var target = ValidateNullable.GetOrThrow(ctx => ctx.Get(input.target));
        using(_scope.WithScope(input))
        using(_elapsed.WithMeter<MediatRController.CalculateInput>())
        {
            return await _unit.DoCalculate(target);
        }
    }
}