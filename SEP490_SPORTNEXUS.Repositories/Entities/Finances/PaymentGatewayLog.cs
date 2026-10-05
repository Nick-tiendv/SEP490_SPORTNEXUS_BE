using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances
{
    public class PaymentGatewayLog : Entity<Guid>
    {
        public Guid TransactionId { get; set; }
        [ForeignKey(nameof(TransactionId))]
        public Transaction Transaction { get; set; } = null!;

        [MaxLength(50)]
        public string Gateway { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public string RawPayload { get; set; } = null!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}