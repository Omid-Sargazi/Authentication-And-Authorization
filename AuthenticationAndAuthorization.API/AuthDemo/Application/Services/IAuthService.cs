using AuthenticationAndAuthorization.API.AuthDemo.Application.Models;

namespace AuthenticationAndAuthorization.API.AuthDemo.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterRequest request);
        Task<AuthResult> LoginRequest(LoginRequest request);
    }
}