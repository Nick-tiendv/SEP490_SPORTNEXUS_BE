using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static SEP490_SPORTNEXUS_BE.Services.RequestModel.AccountRequest;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<IEnumerable<AccountReponse>>> GetAllAsync()
        {
            var accounts = await _context.Accounts
                .Include(a => a.Role)
                .Select(a => new AccountReponse
                {
                    Id = a.Id,
                    Username = a.Username,
                    FullName = a.FullName,
                    RoleId = a.RoleId,
                    RoleName = a.Role.Name,
                    WalletBalance = a.WalletBalance
                })
                .ToListAsync();

            return new ApiResponse<IEnumerable<AccountReponse>>
            {
                StatusCode = 200,
                Message = "Success",
                Data = accounts
            };
        }

        public async Task<ApiResponse<AccountReponse?>> GetByIdAsync(Guid id)
        {
            var a = await _context.Accounts
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (a == null)
            {
                return new ApiResponse<AccountReponse?>
                {
                    StatusCode = 404,
                    Message = "Account not found",
                    Data = null
                };
            }

            var dto = new AccountReponse
            {
                Id = a.Id,
                Username = a.Username,
                FullName = a.FullName,
                RoleId = a.RoleId,
                RoleName = a.Role.Name,
                WalletBalance = a.WalletBalance
            };

            return new ApiResponse<AccountReponse?>
            {
                StatusCode = 200,
                Message = "Success",
                Data = dto
            };
        }

        public async Task<ApiResponse<AccountReponse?>> CreateAsync(CreateAccountRequest request)
        {
            if (await _context.Accounts.AnyAsync(x => x.Username == request.Username))
            {
                return new ApiResponse<AccountReponse?>
                {
                    StatusCode = 400,
                    Message = "Username already exists",
                    Data = null
                };
            }

            var role = await _context.Roles.FindAsync(request.RoleId);
            if (role == null)
            {
                return new ApiResponse<AccountReponse?>
                {
                    StatusCode = 400,
                    Message = "Invalid role",
                    Data = null
                };
            }

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                PasswordHash = HashPassword(request.Password ?? Guid.NewGuid().ToString()),
                FullName = request.FullName,
                RoleId = request.RoleId,
                WalletBalance = request.WalletBalance
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            var dto = new AccountReponse
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                RoleId = account.RoleId,
                RoleName = role.Name,
                WalletBalance = account.WalletBalance
            };

            return new ApiResponse<AccountReponse?>
            {
                StatusCode = 201,
                Message = "Created",
                Data = dto
            };
        }

        public async Task<ApiResponse<AccountReponse?>> UpdateAsync(Guid id, UpdateAccountRequest request)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return new ApiResponse<AccountReponse?>
                {
                    StatusCode = 404,
                    Message = "Account not found",
                    Data = null
                };
            }

            if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != account.Username)
            {
                if (await _context.Accounts.AnyAsync(x => x.Username == request.Username && x.Id != id))
                {
                    return new ApiResponse<AccountReponse?>
                    {
                        StatusCode = 400,
                        Message = "Username already in use",
                        Data = null
                    };
                }
                account.Username = request.Username;
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                account.PasswordHash = HashPassword(request.Password);
            }

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                account.FullName = request.FullName;
            }

            if (request.RoleId.HasValue)
            {
                var role = await _context.Roles.FindAsync(request.RoleId.Value);
                if (role == null)
                {
                    return new ApiResponse<AccountReponse?>
                    {
                        StatusCode = 400,
                        Message = "Invalid role",
                        Data = null
                    };
                }
                account.RoleId = request.RoleId.Value;
            }

            if (request.WalletBalance.HasValue)
            {
                account.WalletBalance = request.WalletBalance.Value;
            }

            await _context.SaveChangesAsync();

            var updated = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Id == id);

            var dto = new AccountReponse
            {
                Id = updated!.Id,
                Username = updated.Username,
                FullName = updated.FullName,
                RoleId = updated.RoleId,
                RoleName = updated.Role.Name,
                WalletBalance = updated.WalletBalance
            };

            return new ApiResponse<AccountReponse?>
            {
                StatusCode = 200,
                Message = "Updated",
                Data = dto
            };
        }

        public async Task<ApiResponse<object?>> DeleteAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return new ApiResponse<object?>
                {
                    StatusCode = 404,
                    Message = "Account not found",
                    Data = null
                };
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return new ApiResponse<object?>
            {
                StatusCode = 200,
                Message = "Deleted",
                Data = null
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
