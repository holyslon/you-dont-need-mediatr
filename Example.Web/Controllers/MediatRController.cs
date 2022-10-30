using System.ComponentModel.DataAnnotations;
using Example.App.MediatR.Calculation;
using Example.App.Validation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ValidationException = Example.App.Validation.ValidationException;

namespace Example.Web.Controllers;

[ApiController]
[Route("mediatr")]
public class MediatRController : ControllerBase
{
    private readonly IMediator _mediator;

    public MediatRController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public record CalculateInput(int? target){}

    [HttpPost("calculate")]
    public async Task<CalculationResponse> Calculate([FromBody] CalculateInput input)
    {
        if (!input.target.HasValue)
        {
            throw new ValidationException(nameof(input.target), "target should present");
        }
        else
        {
            return await _mediator.Send(new CalculationRequest(input.target.Value));
        }
    }
}