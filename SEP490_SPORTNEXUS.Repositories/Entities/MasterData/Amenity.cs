using System;
using System.ComponentModel.DataAnnotations;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData
{
    public class Amenity : Entity<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string? IconCode { get; set; }
    }
}
