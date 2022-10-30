using Example.App.Validation;
using MediatR;

namespace Example.App.MediatR.Calculation;

public class Validator : IPipelineBehavior<CalculationRequest, CalculationResponse>
{
    public Task<CalculationResponse> Handle(CalculationRequest request, RequestHandlerDelegate<CalculationResponse> next, CancellationToken cancellationToken)
    {
        if (request.Target > 19)
        {
            throw new ValidationException(nameof(request.Target), "target more than 19, factorial will overflow");
        }

        return next();
    }
}