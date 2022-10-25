namespace Example.App.Logging;

public class ScopeGeneratorOptions<T>
{
    private readonly IList<IScopeGenerator<T>> _generators = new List<IScopeGenerator<T>>();

    public IEnumerable<IScopeGenerator<T>> Generators => _generators;

    public ScopeGeneratorOptions<T> WithGenerator(Func<T, IReadOnlyDictionary<string, string>> mapper)
    {
        _generators.Add(new ProxyScopeGenerator<T>(mapper));
        return this;
    }
}