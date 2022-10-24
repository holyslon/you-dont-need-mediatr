using Example.App.Native;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers;

[ApiController]
[Route("fsharp")]
public class FSharpController : ControllerBase
{
    private readonly CalculationUnit _unit;

    public FSharpController(CalculationUnit unit)
    {
        _unit = unit;
    }
    
    public record CalculateInput(int? target){}

    [HttpPost("calculate")]
    public async Task<(int,int)> Calculate([FromBody] CalculateInput input)
    {
        if (!input.target.HasValue)
        {
            throw new BadHttpRequestException($"{nameof(MediatRController.CalculateInput.target)} should be present", 400);
        }
        else
        {
            return App.F.Calculator.Calculate(input.target.Value).Value.ToValueTuple();
        }
    }
}