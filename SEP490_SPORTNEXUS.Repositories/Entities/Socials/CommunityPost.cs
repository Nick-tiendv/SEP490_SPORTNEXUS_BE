using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials
{
    public class CommunityPost : Entity<Guid>
    {
        [Required]
        public Guid AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public Account Author { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Column(TypeName = "jsonb")]
        public List<string> MediaUrls { get; set; } = new List<string>();

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        
        public ICollection<PostComment> Comments { get; set; } = new List<PostComment>();
    }
}