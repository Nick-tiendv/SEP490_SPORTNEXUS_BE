using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.System
{
    public class NotificationLog : Entity<Guid>
    {
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public Account User { get; set; } = null!;

        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string Body { get; set; } = null!;

        [MaxLength(50)]
        public string Type { get; set; } = "PUSH";

        public bool IsRead { get; set; } = false;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}