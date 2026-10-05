using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class AccountReponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public decimal FairPlayScore { get; set; }
        public bool IsActive { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public decimal WalletBalance { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
