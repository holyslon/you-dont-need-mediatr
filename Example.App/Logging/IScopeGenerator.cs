namespace Example.App.Logging;

public interface IScopeGenerator<T>
{
    IReadOnlyDictionary<string,string> Generate<TResult>(T request);
}

