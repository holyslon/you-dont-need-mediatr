using System.Diagnostics;

namespace Example.App.Tracing;

public static class Trace
{
    private static readonly ActivitySource ActivitySource = new(nameof(Trace));
    private class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
    public static IDisposable WithTrace<T>()
    {
        var activity = ActivitySource.StartActivity();
        activity?.SetTag("Record", nameof(T));
        return activity as IDisposable ?? new DummyDisposable();
    }

}