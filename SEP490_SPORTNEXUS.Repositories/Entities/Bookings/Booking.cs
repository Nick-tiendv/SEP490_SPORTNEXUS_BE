using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings
{
    public class Booking : Entity<Guid>
    {
        [Required]
        public Guid HostId { get; set; }
        [ForeignKey(nameof(HostId))]
        public Account Host { get; set; } = null!;

        [Required]
        public Guid CourtSlotId { get; set; }
        [ForeignKey(nameof(CourtSlotId))]
        public CourtSlot CourtSlot { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public string? DynamicQRCode { get; set; }

        [Required]
        [MaxLength(20)]
        public BookingStatus Status { get; set; } = BookingStatus.PENDING;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
    }
}