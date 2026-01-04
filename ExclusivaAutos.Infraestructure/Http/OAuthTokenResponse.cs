namespace ExclusivaAutos.Infrastructure.Http
{
    public class OAuthTokenResponse
    {
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }

    }
}
