using MediatR;

namespace Example.App.MediatR.Calculation;

public record CalculationRequest(int Target) : IRequest<CalculationResponse>;