namespace Client;

// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
class ServiceX : IServiceX
{
    private readonly HttpClient _httpClient;

    public ServiceX(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetString()
    {
        Console.WriteLine("ServiceX: Calling external service. This is only one time. Retries and other run inside httpClient.");
        Task<string> t = _httpClient.GetStringAsync("http://localhost:5009/get/success2s");
        await t.ContinueWith((task) => Console.WriteLine($"ServiceX: Got response: {task.Result}"));
        return await t;
    }
}