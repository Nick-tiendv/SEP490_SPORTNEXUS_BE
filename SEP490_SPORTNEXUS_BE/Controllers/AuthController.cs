using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Models;
using SEP490_SPORTNEXUS_BE.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new ApiResponse<object>
                {
                    StatusCode = 400,
                    Message = "Username already exists",
                    Data = null
                });
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                Role = "Player", // Default role
                WalletBalance = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Registration successful",
                Data = null
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            
            if (user == null || user.PasswordHash != HashPassword(request.Password))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    StatusCode = 401,
                    Message = "Invalid username or password",
                    Data = null
                });
            }

            // Mock JWT Token
            var mockToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.MockPayload.MockSignature_" + Guid.NewGuid().ToString("N");

            return Ok(new ApiResponse<object>
            {
                StatusCode = 200,
                Message = "Login successful",
                Data = new 
                {
                    Token = mockToken,
                    FullName = user.FullName
                }
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}

