using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.System
{
    public class AuditLog : Entity<Guid>
    {
        public Guid? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Account? User { get; set; }

        [MaxLength(50)]
        public string Action { get; set; } = null!;

        [MaxLength(100)]
        public string TableName { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public string? OldValues { get; set; }

        [Column(TypeName = "jsonb")]
        public string? NewValues { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}