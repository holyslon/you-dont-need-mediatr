using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Example.App.Logging;

public static class ScopeGeneratorExtensions {
    public static Dictionary<string, string> Generate<T>(this IEnumerable<IScopeGenerator<T>> generators, T state)
    {
        Dictionary<string, string> AggregateDictionaries(Dictionary<string, string> state, IReadOnlyDictionary<string, string> val)
        {
            foreach (var (key, value) in val)
            {
                state.Add(key, value);
            }

            return state;
        }

        return generators.Select(g => g.Generate<T>(state)).Aggregate(new Dictionary<string, string>(),
            AggregateDictionaries);
    }

    public static IServiceCollection AddLoggingScopes(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<IScope, Scope>();
        return serviceCollection;
    }
}