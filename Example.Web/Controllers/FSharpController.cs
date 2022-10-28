using System.Diagnostics;
using Example.App.F;
using Example.App.Native;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;

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

    public record CalculateInput(int? target)
    {
    }

    [HttpPost("calculate")]
    public async Task<(int, int)> Calculate([FromBody] CalculateInput input)
    {
        if (!input.target.HasValue)
        {
            throw new BadHttpRequestException($"{nameof(MediatRController.CalculateInput.target)} should be present",
                400);
        }

        return Calculator.Handle(input.target.Value) switch
        {
            {IsOk: true} result => result.ResultValue.ToValueTuple(),
            var result => result.ErrorValue switch
            {
                Calculator.NumberValidationError.NotPositiveNumberError error => throw new BadHttpRequestException(
                    $"Validation error: {error.Item}",
                    400),
                Calculator.NumberValidationError.ToBigNumberError error => throw new BadHttpRequestException(
                    $"Validation error: {error.Item}",
                    400),
                _ => throw new ArgumentOutOfRangeException(nameof(result.ErrorValue), result.ErrorValue, null)
            }
        };
    }
}