using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CourtId { get; set; }

        [ForeignKey(nameof(CourtId))]
        public Court Court { get; set; } = null!;

        [Required]
        public int PlayerId { get; set; }

        [ForeignKey(nameof(PlayerId))]
        public User Player { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;
    }
}
