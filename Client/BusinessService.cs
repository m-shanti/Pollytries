namespace Client;

class BusinessService : IBusinessService
{
    private readonly IServiceX _serviceX;

    public BusinessService(IServiceX serviceX)
    {
        _serviceX = serviceX;
    }
    
    public Task<string> GetString()
    {
        return _serviceX.GetString();
    }
}