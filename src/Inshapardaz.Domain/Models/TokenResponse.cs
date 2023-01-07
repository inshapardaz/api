namespace Inshapardaz.Domain.Models
{
    public class TokenResponse
    {
        public AccountModel Account { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
