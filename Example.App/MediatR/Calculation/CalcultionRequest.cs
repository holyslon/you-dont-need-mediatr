using MediatR;

namespace Example.App.MediatR.Calculation;

public class CalculationRequest : IRequest<CalculationResponse>
{
    public CalculationRequest(int target)
    {
        Target = target;
    }

    public int Target { get; }
}