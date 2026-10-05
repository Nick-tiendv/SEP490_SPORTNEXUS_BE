using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances
{
    public class Transaction : Entity<Guid>
    {
        [Required]
        public Guid WalletId { get; set; }
        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public TransactionType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public Guid? ReferenceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "SUCCESS";

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}