using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class CheckInLog : Entity<Guid>
    {
        public Guid FacilityId { get; set; }
        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        public Guid? BookingParticipantId { get; set; }
        [ForeignKey(nameof(BookingParticipantId))]
        public BookingParticipant? BookingParticipant { get; set; }

        public Guid? LfgParticipantId { get; set; }
        [ForeignKey(nameof(LfgParticipantId))]
        public LfgParticipant? LfgParticipant { get; set; }

        public Guid? TournamentParticipantId { get; set; }
        [ForeignKey(nameof(TournamentParticipantId))]
        public TournamentParticipant? TournamentParticipant { get; set; }

        public DateTimeOffset CheckedInAt { get; set; } = DateTimeOffset.UtcNow;
    }
}