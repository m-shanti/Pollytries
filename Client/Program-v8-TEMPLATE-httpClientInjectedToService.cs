// // v8 Retry + Circuit Breaker + Fallback + http client injected to service
//
// // dependency injections in console application
// // https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/
//
// using System.Net;
// using Client;
// using Microsoft.Extensions.DependencyInjection;
// using Polly;
// using Polly.CircuitBreaker;
// using Polly.Fallback;
// using Polly.Retry;
//
// ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder = new ResiliencePipelineBuilder<HttpResponseMessage>();
//
// // add http client with policies
// IServiceCollection services = new ServiceCollection();
// services.AddHttpClient<IServiceX, ServiceX>(client =>
//     {
//         client.BaseAddress = new Uri("http://localhost:5009");
//         // This timeout is for all retries together. All periods between retries are counted into timeout.
//         // https://briancaos.wordpress.com/2020/12/16/httpclient-retry-on-http-timeout-with-polly-and-ihttpclientbuilder/
//         // https://github.com/App-vNext/Polly/issues/512
//         // client.Timeout = new TimeSpan(0, 0, 5);        
//     })
//     .AddResilienceHandler("retryCircuitBreakerAndFallbackPipeline",builder =>
//     {
//         //builder.AddSth...
//
//     });
//
// services.AddTransient<IBusinessService, BusinessService>();
//
// ServiceProvider serviceProvider = services.BuildServiceProvider();
//
// IBusinessService? businessService = serviceProvider.GetService<IBusinessService>();
// string result = await businessService.GetString();
// Console.WriteLine(result);