using Example.App.Logging;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Example.App.MediatR.Behaviors.Logging;

[UsedImplicitly]
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;
    private readonly IEnumerable<IScopeGenerator<TRequest>> _requestScopeGenerators;
    private readonly IEnumerable<IScopeGenerator<TResponse>> _responseScopeGenerators;

    public LoggingBehavior(ILogger<TRequest> logger,
        IOptions<ScopeGeneratorOptions<TRequest>> requestOptions,
        IOptions<ScopeGeneratorOptions<TResponse>> responseOptions)
    {
        _logger = logger;
        _requestScopeGenerators = requestOptions.Value?.Generators?? Array.Empty<IScopeGenerator<TRequest>>();
        _responseScopeGenerators = responseOptions.Value?.Generators?? Array.Empty<IScopeGenerator<TResponse>>();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope(_requestScopeGenerators.Generate(request)))
        {
            _logger.LogInformation("Start handling");
            try
            {
                var result = await next();
                using (_logger.BeginScope(_responseScopeGenerators.Generate(result)))
                {
                    _logger.LogInformation("Request handled");
                    return result;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Fail to handle request");
                throw;
            }
        }
    }
}