namespace Example.App.Logging;

public interface IScope
{
    IDisposable WithScope<T>(T state);
}