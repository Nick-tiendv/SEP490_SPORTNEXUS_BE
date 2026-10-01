using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class Account : Entity<Guid>
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
        [Column(TypeName = "decimal(18,2)")]
        public decimal WalletBalance { get; set; }
    }
}
