using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class FacilityImage : Entity<Guid>
    {
        [Required]
        public Guid FacilityId { get; set; }

        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;

        public bool IsThumbnail { get; set; } = false;
    }
}
