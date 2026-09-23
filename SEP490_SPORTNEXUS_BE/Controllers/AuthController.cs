using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using System.Security.Cryptography;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}

