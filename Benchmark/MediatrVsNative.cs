using BenchmarkDotNet.Attributes;

namespace Benchmark;

public class MediatrVsNative
{
    private  IHos? _mediatrServiceProvider;
    private  IServiceProvider? _nativeServiceProvider;


    [GlobalSetup]
    public void GlobalSetup()
    {
    }
    
    [GlobalCleanup]
    public void GlobalCleanup()
    {
        _mediatrServiceProvider?.
    }

}