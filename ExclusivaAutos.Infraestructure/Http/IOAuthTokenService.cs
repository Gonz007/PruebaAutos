namespace ExclusivaAutos.Infrastructure.Http
{
    public interface IOAuthTokenService
    {
        Task<string> GetAccessTokenAsync();
    }
}
