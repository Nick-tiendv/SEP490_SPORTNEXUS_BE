using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class Booking : Entity<Guid>
    {
        public Guid CourtId { get; set; }
        public Court Court { get; set; } = null!;
        public Guid PlayerId { get; set; }
        public Account Player { get; set; } = null!;
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;
    }
}
