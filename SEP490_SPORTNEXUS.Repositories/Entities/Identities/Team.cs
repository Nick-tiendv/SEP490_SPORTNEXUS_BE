using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities
{
    public class Team : Entity<Guid>, IAuditable
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public Account CreatedBy { get; set; } = null!;

        // Navigation property for Team Members
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();

        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ModifiedAt { get; set; }
    }
}

