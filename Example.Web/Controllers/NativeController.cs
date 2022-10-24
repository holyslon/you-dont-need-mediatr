using Example.App.MediatR.Calculation;
using Example.App.Native;
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
    
    public record CalculateInput(int? target){}

    [HttpPost("calculate")]
    public async Task<(int,int)> Calculate([FromBody] CalculateInput input)
    {
        if (!input.target.HasValue)
        {
            throw new BadHttpRequestException($"{nameof(MediatRController.CalculateInput.Target)} should be present", 400);
        }
        else
        {
            return await _unit.DoCalulate(input.target.Value);
        }
    }
}