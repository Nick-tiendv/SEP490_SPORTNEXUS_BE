using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments
{
    public class Tournament : Entity<Guid>
    {
        public Guid FacilityId { get; set; }
        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public Guid? ParentTournamentId { get; set; }
        [ForeignKey(nameof(ParentTournamentId))]
        public Tournament? ParentTournament { get; set; }

        [Required]
        [MaxLength(20)]
        public TournamentCategoryType CategoryType { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string Format { get; set; } = "Knockout";

        public int MaxParticipants { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EntryFee { get; set; }

        [Required]
        [MaxLength(20)]
        public TournamentStatus Status { get; set; } = TournamentStatus.OPEN;

        public ICollection<TournamentPrize> Prizes { get; set; } = new List<TournamentPrize>();
        public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
    }
}