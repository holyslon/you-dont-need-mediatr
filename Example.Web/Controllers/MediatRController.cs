using Example.App.MediatR.Calculation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    
    public record CalculateInput(int? Target){}

    [HttpPost("calculate")]
    public async Task<CalculationResponse> Calculate([FromBody] CalculateInput input)
    {
        if (!input.Target.HasValue)
        {
            throw new BadHttpRequestException($"{nameof(input.Target)} should be present", 400);
        }
        else
        {
            return await _mediator.Send(new CalculationRequest(input.Target.Value));
        }
    }
}