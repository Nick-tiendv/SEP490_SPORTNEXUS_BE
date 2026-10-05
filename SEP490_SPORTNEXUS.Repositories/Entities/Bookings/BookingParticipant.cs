using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings
{
    public class BookingParticipant : Entity<Guid>
    {
        [Required]
        public Guid BookingId { get; set; }
        [ForeignKey(nameof(BookingId))]
        public Booking Booking { get; set; } = null!;

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Account User { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountContributed { get; set; }

        [Required]
        [MaxLength(20)]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;
        public string? QrCode { get; set; }
        public bool IsCheckedIn { get; set; } = false;
    }
}