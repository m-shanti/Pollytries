// v8 RateLimiter + http client injected to service

// change endpoint in ServiceX.cs to http://localhost:5009/get/success2s

// dependency injections in console application
// https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/

using System.Net;
using System.Threading.RateLimiting;
using Client;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;

ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder = new ResiliencePipelineBuilder<HttpResponseMessage>();

// add http client with policies
IServiceCollection services = new ServiceCollection();
services.AddHttpClient<IServiceX, ServiceX>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5009");
        // This timeout is for all retries together. All periods between retries are counted into timeout.
        // https://briancaos.wordpress.com/2020/12/16/httpclient-retry-on-http-timeout-with-polly-and-ihttpclientbuilder/
        // https://github.com/App-vNext/Polly/issues/512
        // client.Timeout = new TimeSpan(0, 0, 5);        
    })
    .AddResilienceHandler("retryCircuitBreakerAndFallbackPipeline",builder =>
    {
        builder.AddRateLimiter(new ConcurrencyLimiter(new ConcurrencyLimiterOptions()
            {
                PermitLimit = 3,
                QueueLimit = 50
            }
        ));
    });

services.AddTransient<IBusinessService, BusinessService>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

Parallel.For(0,50, new ParallelOptions()
    {
        MaxDegreeOfParallelism = 1000
    },
    i =>
    {
        IBusinessService? businessService = serviceProvider.GetService<IBusinessService>();
        string result = businessService.GetString().Result;
        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} result");
    });