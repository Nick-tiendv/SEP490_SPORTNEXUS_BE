using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments
{
    public class TournamentParticipant : Entity<Guid>
    {
        public Guid TournamentId { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; } = null!;

        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Account? User { get; set; }

        public Guid? TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team? Team { get; set; }

        [MaxLength(20)]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;
        public string? QrCode { get; set; }
        public bool IsCheckedIn { get; set; } = false;
    }
}