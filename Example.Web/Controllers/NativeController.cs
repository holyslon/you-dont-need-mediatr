using Example.App.Native;
using Example.Web.Validation;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

[ApiController]
[Route("native")]
public class NativeController : ControllerBase
{
    private readonly CalculationUnit _unit;

    public NativeController(CalculationUnit unit)
    {
        _unit = unit;
    }

    public record CalculateInput(int? target);

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input)
    {
        var target = ValidateNullable.GetOrThrow(ctx => ctx.Get(input.target));
        return await _unit.DoCalculate(target);
    }
}