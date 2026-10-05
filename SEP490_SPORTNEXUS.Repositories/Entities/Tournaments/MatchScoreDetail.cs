using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments
{
    public class MatchScoreDetail : Entity<Guid>
    {
        public Guid BracketMatchId { get; set; }
        [ForeignKey(nameof(BracketMatchId))]
        public BracketMatch BracketMatch { get; set; } = null!;

        public int SetNumber { get; set; }
        public int ScoreP1 { get; set; }
        public int ScoreP2 { get; set; }
    }
}