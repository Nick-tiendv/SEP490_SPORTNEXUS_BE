using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SEP490_SPORTNEXUS_BE.Services.IServices; // add if Task isn't already in scope

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context; // <-- replace YourDbContext with your actual DbContext type

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse<object?>> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Accounts.AnyAsync(a => a.Username == request.Username))
            {
                return new ApiResponse<object?>
                {
                    StatusCode = 400,
                    Message = "Username already exists",
                    Data = null
                };
            }
            var playerRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == RoleIds.Player || r.Name == "Player");
            if (playerRole == null)
            {
                return new ApiResponse<object?>
                {
                    StatusCode = 500,
                    Message = "Default role is not configured",
                    Data = null
                };
            }
            _context.Accounts.Add(new Account
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = HashPassword(request.Password),
                FullName = request.FullName,
                RoleId = playerRole.Id,
                WalletBalance = 0
            });
            await _context.SaveChangesAsync();
            return new ApiResponse<object?>
            {
                StatusCode = 200,
                Message = "Registration successful",
                Data = null
            };
        }
        public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request)
        {
            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Username == request.Username);
            if (account == null || account.PasswordHash != HashPassword(request.Password))
            {
                return new ApiResponse<LoginResponse?>
                {
                    StatusCode = 401,
                    Message = "Invalid username or password",
                    Data = null
                };
            }
            var mockToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.MockPayload.MockSignature_"
                            + Guid.NewGuid().ToString("N");
            return new ApiResponse<LoginResponse?>
            {
                StatusCode = 200,
                Message = "Login successful",
                Data = new LoginResponse { Token = mockToken, FullName = account.FullName }
            };
        }
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
