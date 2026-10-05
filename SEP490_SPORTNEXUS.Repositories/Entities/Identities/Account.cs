using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities
{
    public class Account : Entity<Guid>, IAuditable
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        public string? AvatarUrl { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal FairPlayScore { get; set; } = 100.0m;

        public bool IsActive { get; set; } = true;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal WalletBalance { get; set; }

        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}
