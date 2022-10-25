using Example.App.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Example.App.Metrics;

public static class ElapsedExtensions {

    public static IServiceCollection AddElapsedMetric(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IElapsed, Elapsed>();
        return serviceCollection;
    }
}