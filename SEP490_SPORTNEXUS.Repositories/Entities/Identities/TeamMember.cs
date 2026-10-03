using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities
{
    public class TeamMember : Entity<Guid>
    {
        [Required]
        public Guid TeamId { get; set; }

        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; } = null!;

        [Required]
        public Guid AccountId { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account Account { get; set; } = null!;

        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

