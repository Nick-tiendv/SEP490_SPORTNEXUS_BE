using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials
{
    public class FairPlayRating : Entity<Guid>
    {
        public Guid ReviewerId { get; set; }
        [ForeignKey(nameof(ReviewerId))]
        public Account Reviewer { get; set; } = null!;

        public Guid RevieweeId { get; set; }
        [ForeignKey(nameof(RevieweeId))]
        public Account Reviewee { get; set; } = null!;

        public Guid ContextId { get; set; } // BookingId or LfgCardId

        [Range(1, 5)]
        public int Score { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}