using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public static class AccountRequest
    {
        public class CreateAccountRequest
        {
            public string Username { get; set; } = null!;
            public string? Password { get; set; } // optional, service will generate if null
            public string FullName { get; set; } = null!;
            public string Phone { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string? AvatarUrl { get; set; }
            public decimal FairPlayScore { get; set; } = 100.0m;
            public bool IsActive { get; set; } = true;
            public Guid RoleId { get; set; }
            public decimal WalletBalance { get; set; } = 0;
        }

        public class UpdateAccountRequest
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
            public string? FullName { get; set; }
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? AvatarUrl { get; set; }
            public decimal? FairPlayScore { get; set; }
            public bool? IsActive { get; set; }
            public Guid? RoleId { get; set; }
            public decimal? WalletBalance { get; set; }
        }
    }
}
