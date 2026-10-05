using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials
{
    public class LfgCard : Entity<Guid>
    {
        [Required]
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        public Guid? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }

        [MaxLength(100)]
        public string? SkillLevelTag { get; set; }

        [Required]
        public int SlotsNeeded { get; set; }

        [Required]
        [MaxLength(20)]
        public LfgCardStatus Status { get; set; } = LfgCardStatus.PENDING;

        public ICollection<LfgParticipant> Participants { get; set; } = new List<LfgParticipant>();
    }
}