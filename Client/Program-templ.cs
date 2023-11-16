// // v8 Retry + circuit breaker + fallback + http client injected to service
//
// // See https://aka.ms/new-console-template for more information
//
// // dependency injections
// // https://andrewlock.net/using-dependency-injection-in-a-net-core-console-application/
//
// using System.Diagnostics;
// using Client;
// using Microsoft.Extensions.DependencyInjection;
// using Polly;
// using Polly.CircuitBreaker;
// using Polly.Extensions.Http;
// using Polly.Fallback;
// using Polly.Retry;
// using Polly.Timeout;
// using Sundry.Extensions.Http.Polly.DependencyInjection;
//
// ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder = new ResiliencePipelineBuilder<HttpResponseMessage>();
//
// // circuit breaker
// // pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
// // {
// //     ShouldHandle = new PredicateBuilder<HttpResponseMessage>().Handle<Exception>(),
// //     FailureRatio = 0.5d,
// //     SamplingDuration = TimeSpan.FromSeconds(5),
// //     MinimumThroughput = 2,
// //     BreakDuration = TimeSpan.FromSeconds(1),
// //     OnOpened = _ =>
// //     {
// //         Console.WriteLine("Circuit opened.");
// //         return ValueTask.CompletedTask;
// //     },
// //     OnClosed = _ =>
// //     {
// //         Console.WriteLine("Circuit opened.");
// //         return ValueTask.CompletedTask;
// //     }
// // });
//
// // retry
// pipelineBuilder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>()
// {
//     ShouldHandle = new PredicateBuilder<HttpResponseMessage>() // why it didn't work when I had <Exception>
//         .Handle<HttpRequestException>()
//         .HandleResult(result => !result.IsSuccessStatusCode), // lack of it is a common mistake
//     Delay = TimeSpan.FromSeconds(1),
//     OnRetry = arg =>
//     {
//         Console.WriteLine($"Failed. Retry in {arg.RetryDelay.TotalSeconds} seconds...");
//         return ValueTask.CompletedTask;
//     },
//     MaxRetryAttempts = 10 // interesting, default is 3, "do trzech razy sztuka", 3 times a charm
// });
//
//
// // // build retry policy
// // AsyncRetryPolicy<HttpResponseMessage> retry = Policy
// //     .Handle<Exception>(exception => exception is not BrokenCircuitException)
// //     .OrTransientHttpStatusCode()
// //     .WaitAndRetryForeverAsync(retryAttempt => TimeSpan.FromSeconds(1),
// //         (result, i, timespan) => { Console.WriteLine($"Failed call. Next retry after {timespan.TotalSeconds} seconds ...");});
// //
// // // build timeout policy
// // AsyncTimeoutPolicy timeout = Policy.TimeoutAsync(
// //     TimeSpan.FromSeconds(5),
// //     TimeoutStrategy.Optimistic,
// //     async (context, timespan, task) => { });
// //
// // // build circuit breaker policy
// // AsyncCircuitBreakerPolicy<HttpResponseMessage> circuitBreaker = HttpPolicyExtensions
// //     .HandleTransientHttpError()
// //     .CircuitBreakerAsync(3, TimeSpan.FromSeconds(5),
// //         (exception, timespan, context) => Console.WriteLine($"Breaking circuit for {timespan.TotalSeconds}!"),
// //         (context) => { });
//
// // IAsyncPolicy<HttpResponseMessage> fallbackPolicy =
// //     Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
// //         .FallbackAsync(FallbackAction, OnFallbackAsync);
// //
// // AsyncFallbackPolicy<HttpResponseMessage> asyncFallbackPolicy = Policy
// //     .Handle<TimeoutRejectedException>()
// //     .OrTransientHttpError()
// //     .FallbackAsync(async (cancellationToken) => Console.WriteLine("sdf"));
// //
// // // build fallback policy
// // FallbackPolicy<HttpResponseMessage> fallback = Policy.Handle<BrokenCircuitException>()
// //     .OrTransientHttpError() // remove
// //     .Fallback();
//
// // TODD handled event in period
// // onopen/close
//
// // build pipeline
// ResiliencePipeline<HttpResponseMessage> pipeline = pipelineBuilder.Build();
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
//     .AddResiliencePipelineHandler(pipeline);
//
// services.AddTransient<IBusinessService, BusinessService>();
//
// ServiceProvider serviceProvider = services.BuildServiceProvider();
//
// IBusinessService? businessService = serviceProvider.GetService<IBusinessService>();
// string result = await businessService.GetString();
// Console.WriteLine(result);