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
//         builder
//             // fallback
//             .AddFallback(new FallbackStrategyOptions<HttpResponseMessage>()
//                 {
//                     ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
//                         .Handle<BrokenCircuitException>(), // explain why not BrokenCircuitException
//                     OnFallback = _ =>
//                     {
//                         Console.WriteLine("Fallback called.");
//                         return ValueTask.CompletedTask;
//                     },
//                     FallbackAction = _ =>
//                     {
//                         // artificial OK
//                         return Outcome.FromResultAsValueTask(new HttpResponseMessage(HttpStatusCode.OK)
//                         {
//                             Content = new StringContent("Artificial OK (when circuit is open)")
//                         });
//                         
//                     }
//                 }
//             )
//             // retry
//             .AddRetry(new RetryStrategyOptions<HttpResponseMessage>()
//             {
//                 ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
//                     .Handle<HttpRequestException>()
//                     .HandleResult(result => !result.IsSuccessStatusCode), // lack of it is a common mistake
//                 BackoffType = DelayBackoffType.Constant,
//                 Delay = TimeSpan.FromSeconds(0.25),
//                 UseJitter = false,
//                 OnRetry = arg =>
//                 {
//                     Console.WriteLine($"Failed. Retry in {arg.RetryDelay.TotalSeconds} seconds...");
//                     return ValueTask.CompletedTask;
//                 },
//                 MaxRetryAttempts = Int32.MaxValue
//             })
//             // circuit breaker
//             .AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
//             {
//                 ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
//                     .HandleResult(result => !result.IsSuccessStatusCode),
//                 FailureRatio = 0.5d,
//                 SamplingDuration = TimeSpan.FromSeconds(5),
//                 MinimumThroughput = 5,
//                 BreakDuration = TimeSpan.FromSeconds(5),
//                 OnOpened = _ =>
//                 {
//                     Console.WriteLine("Circuit opened for 5 seconds...");
//                     return ValueTask.CompletedTask;
//                 },
//                 OnClosed = _ =>
//                 {
//                     Console.WriteLine("Circuit closed again.");
//                     return ValueTask.CompletedTask;
//                 }
//             });
//     });
//
// services.AddTransient<IBusinessService, BusinessService>();
//
// ServiceProvider serviceProvider = services.BuildServiceProvider();
//
// IBusinessService? businessService = serviceProvider.GetService<IBusinessService>();
// string result = await businessService.GetString();
// Console.WriteLine(result);