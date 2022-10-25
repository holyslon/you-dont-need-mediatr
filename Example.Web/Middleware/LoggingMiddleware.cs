using Example.App.Logging;
using Example.App.MediatR.Behaviors.Logging;
using Microsoft.Extensions.Options;

namespace Example.Web.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly IEnumerable<IScopeGenerator<HttpRequest>> _requestGenerator;
    private readonly IEnumerable<IScopeGenerator<HttpResponse>> _responseGenerator;

    public LoggingMiddleware(RequestDelegate next, 
        IOptions<ScopeGeneratorOptions<HttpRequest>> requestGeneratorOptions,
        IOptions<ScopeGeneratorOptions<HttpResponse>> responseGeneratorOptions, 
        ILogger<LoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        if (requestGeneratorOptions == null)
        {
            throw new ArgumentNullException(nameof(requestGeneratorOptions));
        }

        _requestGenerator = requestGeneratorOptions.Value?.Generators?? Array.Empty<IScopeGenerator<HttpRequest>>();
        _responseGenerator = responseGeneratorOptions.Value?.Generators?? Array.Empty<IScopeGenerator<HttpResponse>>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task Invoke(HttpContext context)
    {
        using (_logger.BeginScope(_requestGenerator.Generate(context.Request)))
        {
            _logger.LogInformation("Start handling http request");
            try
            {
                await _next(context);
                
                using (_logger.BeginScope(_responseGenerator.Generate(context.Response)))
                {
                    _logger.LogInformation("Http request handled");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Fail to handle http request");
                throw;
            }
        }
    }
}