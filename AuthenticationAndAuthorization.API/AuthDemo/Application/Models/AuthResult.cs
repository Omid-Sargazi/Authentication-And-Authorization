namespace AuthenticationAndAuthorization.API.AuthDemo.Application.Models
{
    public class AuthResult
    {
        public string Token { get; set; } = null;
        public DateTime ExpiresAt { get; set; }
    }
}