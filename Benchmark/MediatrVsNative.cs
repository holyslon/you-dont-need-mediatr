using BenchmarkDotNet.Attributes;
using Example.App;
using Example.App.MediatR.Calculation;
using Example.App.Native;
using Example.App.Services.Calculation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmark;

[MemoryDiagnoser]
[CsvExporter]
[CsvMeasurementsExporter]
public class MediatrVsNative
{
    private IServiceProvider? _mediatrServiceProviderTransient;
    private IServiceProvider? _mediatrAsSingletonServiceProviderTransient;
    private IServiceProvider? _nativeServiceProviderTransient;
    private IServiceProvider? _mediatrServiceProviderSingleton;
    private IServiceProvider? _mediatrAsSingletonServiceProviderSingleton;
    private IServiceProvider? _nativeServiceProviderSingleton;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _mediatrServiceProviderTransient = ServicesTransient().AddMediatR(typeof(App)).BuildServiceProvider();
        _mediatrAsSingletonServiceProviderTransient = ServicesTransient()
            .AddMediatR(conf => conf.AsSingleton(), typeof(App))
            .BuildServiceProvider();
        _nativeServiceProviderTransient = ServicesTransient().AddTransient<CalculationUnit>().BuildServiceProvider();
        _mediatrServiceProviderSingleton = ServicesSingleton().AddMediatR(typeof(App)).BuildServiceProvider();
        _mediatrAsSingletonServiceProviderSingleton = ServicesSingleton()
            .AddMediatR(conf => conf.AsSingleton(), typeof(App))
            .BuildServiceProvider();
        _nativeServiceProviderSingleton = ServicesSingleton().AddTransient<CalculationUnit>().BuildServiceProvider();
    }

    private static ServiceCollection ServicesTransient()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient<FactorialService>();
        serviceCollection.AddTransient<FibonacciService>();
        return serviceCollection;
    }

    private static ServiceCollection ServicesSingleton()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<FactorialService>();
        serviceCollection.AddSingleton<FibonacciService>();
        return serviceCollection;
    }

    [Benchmark(Baseline = true)]
    public (int, int) JustCall()
    {
        var factorialService = _nativeServiceProviderSingleton.GetService<FactorialService>();
        var fibonacciService = _nativeServiceProviderSingleton.GetService<FibonacciService>();

        return (factorialService.GetFactorial(7), fibonacciService.GetNumber(7));
    }

    [Benchmark]
    public Task<(int, int)> NativeTransient() =>
        _nativeServiceProviderTransient.GetService<CalculationUnit>().DoCalulate(7);

    [Benchmark]
    public Task<CalculationResponse> MediatrTransient() =>
        _mediatrServiceProviderTransient.GetService<IMediator>().Send(new CalculationRequest(7));
    
    [Benchmark]
    public Task<CalculationResponse> MediatrAsSingletonTransient() =>
        _mediatrAsSingletonServiceProviderTransient.GetService<IMediator>().Send(new CalculationRequest(7));

    [Benchmark]
    public Task<(int, int)> NativeSingleton() =>
        _nativeServiceProviderSingleton.GetService<CalculationUnit>().DoCalulate(7);

    [Benchmark]
    public Task<CalculationResponse> MediatrSingleton() =>
        _mediatrServiceProviderSingleton.GetService<IMediator>().Send(new CalculationRequest(7));

    [Benchmark]
    public Task<CalculationResponse> MediatrAsSingletonSingleton() =>
        _mediatrAsSingletonServiceProviderSingleton.GetService<IMediator>().Send(new CalculationRequest(7));


    [GlobalCleanup]
    public void GlobalCleanup()
    {
    }
}