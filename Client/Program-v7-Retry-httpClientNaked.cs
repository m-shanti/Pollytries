// // v7 Retry + named http client and IHttpClientFactory
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
// using Polly.Extensions.Http;
// using Polly.Retry;
// using Polly.Timeout;
//
// // build retry policy
// AsyncRetryPolicy<HttpResponseMessage> retry = HttpPolicyExtensions
//     .HandleTransientHttpError()
//     .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
//         (result, timespan, context) => { Console.WriteLine($"Failed call. Next retry after {timespan.TotalSeconds} seconds ...");});
//
// // build timeout policy
// AsyncTimeoutPolicy timeout = Policy.TimeoutAsync(
//     TimeSpan.FromSeconds(5),
//     TimeoutStrategy.Optimistic,
//     async (context, timespan, task) => { });
//
// // add http client with policies
// IServiceCollection services = new ServiceCollection();
// services.AddHttpClient("MyService", client =>
//     {
//         client.BaseAddress = new Uri("http://localhost:5009");
//         // This timeout is for all retries together. All periods between retries are counted into timeout.
//         // https://briancaos.wordpress.com/2020/12/16/httpclient-retry-on-http-timeout-with-polly-and-ihttpclientbuilder/
//         // https://github.com/App-vNext/Polly/issues/512
//         // client.Timeout = new TimeSpan(0, 0, 5);
//     })
//     .AddPolicyHandler(retry.WrapAsync(timeout));
//
// ServiceProvider serviceProvider = services.BuildServiceProvider();
//
// IHttpClientFactory httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
// HttpClient httpClient = httpClientFactory.CreateClient("MyService");
// string result = await httpClient.GetStringAsync("/gremlin/get");
//
// Console.WriteLine(result);
//
//
//
//
//
//
// // services.AddHttpClient<IServiceX, ServiceX>()
// //     .AddPolicyHandler(retry.WrapAsync(timeout));
// // services.AddTransient<IBusinessService, BusinessService>();
// //
// // IBusinessService? businessService = serviceProvider.GetService<IBusinessService>();
// //
// // Console.WriteLine(await businessService.GetString());
//
