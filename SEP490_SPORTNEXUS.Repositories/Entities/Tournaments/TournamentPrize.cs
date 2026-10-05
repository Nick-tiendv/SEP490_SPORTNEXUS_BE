using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments
{
    public class TournamentPrize : Entity<Guid>
    {
        public Guid TournamentId { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; } = null!;

        [MaxLength(50)]
        public string Position { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RewardAmount { get; set; }
    }
}