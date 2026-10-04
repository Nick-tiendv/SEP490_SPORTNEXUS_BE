using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class CourtSlot : Entity<Guid>
    {
        [Required]
        public Guid CourtId { get; set; }

        [ForeignKey(nameof(CourtId))]
        public Court Court { get; set; } = null!;

        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public CourtSlotStatus Status { get; set; } = CourtSlotStatus.Available;
    }
}
