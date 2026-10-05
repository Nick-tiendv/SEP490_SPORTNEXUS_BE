using System;
using System.ComponentModel.DataAnnotations;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData
{
    public class SportCategory : Entity<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? RulesDescription { get; set; }
    }
}
