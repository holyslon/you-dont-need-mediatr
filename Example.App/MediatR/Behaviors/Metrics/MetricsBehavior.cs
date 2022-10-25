using System.Diagnostics;
using System.Diagnostics.Metrics;
using MediatR;

namespace Example.App.MediatR.Behaviors.Metrics;

public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private static readonly Meter Meter = new Meter(typeof(MetricsBehavior<,>).FullName!);
    private static Histogram<long> _time;
    private static readonly Counter<int> _calls;
    private static readonly Counter<int> _success;
    private static readonly Counter<int> _errors;

    private static readonly KeyValuePair<string, object?> RequestTag = new("request", nameof(TRequest));

    static MetricsBehavior()
    {
        _time = Meter.CreateHistogram<long>("Time");
        _calls = Meter.CreateCounter<int>("Calls");
        _success = Meter.CreateCounter<int>("Success");
        _errors = Meter.CreateCounter<int>("Error");
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _calls.Add(1,RequestTag);
        try
        {
            var response = await next();
            _success.Add(1,RequestTag);
            return response;
        }
        catch
        {
            _errors.Add(1,RequestTag);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            _time.Record(stopwatch.ElapsedMilliseconds,RequestTag);
        }
    }
}