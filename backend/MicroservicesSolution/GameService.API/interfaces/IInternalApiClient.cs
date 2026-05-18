namespace GameService.API.interfaces
{
    public interface IInternalApiClient
    {
        Task<T?> GetAsync<T>(string url);
    }
}
