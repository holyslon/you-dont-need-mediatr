using System.Diagnostics;
using System.Diagnostics.Metrics;
using Example.App.MediatR.Behaviors.Metrics;

namespace Example.App.Metrics;
public class Elapsed
{
    private static readonly Meter Meter = new Meter(nameof(Elapsed));
    private static Histogram<long> _time;
    
    static Elapsed()
    {
        _time = Meter.CreateHistogram<long>("Time");
    }

    private class DisposableMeter : IDisposable
    {
        private readonly KeyValuePair<string, object?> _requestTag;
        private readonly Histogram<long> _time;
        private Stopwatch _startNew;

        public DisposableMeter(KeyValuePair<string,object?> requestTag, Histogram<long> time)
        {
            _requestTag = requestTag;
            _time = time;
            _startNew = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _startNew.Stop();
            _time.Record(_startNew.ElapsedMilliseconds, _requestTag);
        }
    }
    public static IDisposable WithMeter<T>()
    {
        KeyValuePair<string, object?> requestTag = new("request", nameof(T));

        return new DisposableMeter(requestTag, _time);

    }
}