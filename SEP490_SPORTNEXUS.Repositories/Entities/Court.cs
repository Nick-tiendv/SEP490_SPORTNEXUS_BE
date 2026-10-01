using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class Court : Entity<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerHour { get; set; }
    }
}
