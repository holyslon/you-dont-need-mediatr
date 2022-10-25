namespace Example.App.Logging;

internal class ProxyScopeGenerator<T> : IScopeGenerator<T>
{
    private readonly Func<T, IReadOnlyDictionary<string, string>> _mapper;

    public ProxyScopeGenerator(Func<T, IReadOnlyDictionary<string, string>> mapper)
    {
        _mapper = mapper;
    }

    public IReadOnlyDictionary<string, string> Generate<TResult>(T request) => _mapper(request);
}