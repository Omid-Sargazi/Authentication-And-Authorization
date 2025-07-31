using AuthenticationAndAuthorization.API.AuthDemo.Application.Models;
using AuthenticationAndAuthorization.API.AuthDemo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAndAuthorization.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest register)
        {
            var result = await  _authService.RegisterAsync(register);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
    }
}