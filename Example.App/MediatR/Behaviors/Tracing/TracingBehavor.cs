using System.Diagnostics;
using MediatR;

namespace Example.App.MediatR.Behaviors.Tracing;

public class TracingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private static readonly ActivitySource ActivitySource = new(typeof(TracingBehavior<,>).FullName!);
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity(ActivityKind.Internal);
        activity?.SetTag("Request", nameof(TRequest));
        return await next();
    }
}