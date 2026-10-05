using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials
{
    public class LfgParticipant : Entity<Guid>
    {
        [Required]
        public Guid LfgCardId { get; set; }
        [ForeignKey(nameof(LfgCardId))]
        public LfgCard LfgCard { get; set; } = null!;

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Account? User { get; set; }

        public Guid? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }

        [Required]
        [MaxLength(20)]
        public LfgParticipantStatus Status { get; set; } = LfgParticipantStatus.PENDING;
        public string? QrCode { get; set; }
        public bool IsCheckedIn { get; set; } = false;
    }
}