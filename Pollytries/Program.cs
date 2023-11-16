using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// builder.Services.AddHttpClient<>()
//
// // approach 1, AddPolicyHandler with Microsoft.Extensions.Http.Polly
// builder.Services
//     .AddHttpClient<IServiceX, ServiceX>()
//     .AddPolicyHandler(GetRetryPolicy()); // Microsoft.Extensions.Http.Polly

WebApplication app = builder.Build();

// IServiceX serviceX = app.Services.GetService<IServiceX>();
// string result = await serviceX.GetString();

app.MapGet("/get/fail", () => Results.Problem("Problem"));
app.MapGet("/get/success", () => Results.Ok("OK"));
app.MapGet("/get/failorsuccess", () =>
{
    Random random = new Random();
    int next = random.Next(1, 10);
    if (next <= 5)
    {
        return Results.Ok("OK");
    }
    return Results.Problem("Problem");
});

app.Run();

// static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
// {
//     return HttpPolicyExtensions
//         .HandleTransientHttpError()
//         .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
//         .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
// }
//
//
//
// // approach 2, pipeline, Polly.Core
// ResiliencePipelineBuilder strategy = new ResiliencePipelineBuilder();
// strategy.AddRetry(new RetryStrategyOptions()
// {
//     ShouldHandle = new PredicateBuilder().Handle<Exception>(),
//     MaxRetryAttempts = 3,
//     Delay = TimeSpan.FromMilliseconds(200), // Wait between each try
//     OnRetry = args =>
//     {
//         return default;
//     }
// });
//strategy.Build();

