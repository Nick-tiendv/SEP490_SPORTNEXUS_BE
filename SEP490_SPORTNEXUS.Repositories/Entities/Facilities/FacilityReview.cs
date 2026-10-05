using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class FacilityReview : Entity<Guid>
    {
        [Required]
        public Guid FacilityId { get; set; }

        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Account User { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
