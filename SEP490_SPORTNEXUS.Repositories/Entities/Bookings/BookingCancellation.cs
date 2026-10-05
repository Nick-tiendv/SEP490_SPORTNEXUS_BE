using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings
{
    public class BookingCancellation : Entity<Guid>
    {
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        public Guid CancelledBy { get; set; }
        [ForeignKey(nameof(CancelledBy))]
        public Account Account { get; set; } = null!;

        public string Reason { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PenaltyAmount { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}