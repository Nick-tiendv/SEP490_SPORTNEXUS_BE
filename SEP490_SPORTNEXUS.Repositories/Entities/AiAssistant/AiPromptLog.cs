using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.AiAssistant
{
    public class AiPromptLog : Entity<Guid>
    {
        public Guid SessionId { get; set; }
        [ForeignKey(nameof(SessionId))]
        public AiChatSession Session { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Sender { get; set; } = null!; // USER or BOT

        [Required]
        public string Message { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public string? IntentExtracted { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}