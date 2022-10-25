using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Example.App.Logging;

internal class Scope: IScope
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Scope> _logger;


    public Scope(IServiceProvider serviceProvider, ILogger<Scope> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public IDisposable WithScope<T>(T state)
    {
        var options = _serviceProvider.GetService<IOptions<ScopeGeneratorOptions<T>>>();
        if (options == null || options.Value == null || !options.Value.Generators.Any())
        {
            return new DummyDisposable();
        }

        return _logger.BeginScope(options.Value.Generators.Generate(state));
    }
}