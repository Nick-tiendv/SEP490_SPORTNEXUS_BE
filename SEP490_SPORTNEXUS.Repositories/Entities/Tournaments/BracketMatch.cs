using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments
{
    public class BracketMatch : Entity<Guid>
    {
        public Guid TournamentId { get; set; }
        [ForeignKey(nameof(TournamentId))]
        public Tournament Tournament { get; set; } = null!;

        public Guid? CourtSlotId { get; set; }
        [ForeignKey(nameof(CourtSlotId))]
        public CourtSlot? CourtSlot { get; set; }

        [MaxLength(50)]
        public string RoundName { get; set; } = null!;

        public int MatchIndex { get; set; } // The Binary Tree Index (1 = Final, 2 = Semifinal 1, 3 = Semifinal 2...)

        public Guid? Participant1Id { get; set; }
        [ForeignKey(nameof(Participant1Id))]
        public TournamentParticipant? Participant1 { get; set; }

        public Guid? Participant2Id { get; set; }
        [ForeignKey(nameof(Participant2Id))]
        public TournamentParticipant? Participant2 { get; set; }

        public Guid? WinnerId { get; set; }
        [ForeignKey(nameof(WinnerId))]
        public TournamentParticipant? Winner { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "SCHEDULED";

        public ICollection<MatchScoreDetail> Scores { get; set; } = new List<MatchScoreDetail>();
    }
}