using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class Court : Entity<Guid>
    {
        [Required]
        public Guid FacilityId { get; set; }

        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public SportCategory Category { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DefaultPrice { get; set; }
    }
}
