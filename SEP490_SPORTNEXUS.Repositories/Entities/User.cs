using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal WalletBalance { get; set; }
    }
}
