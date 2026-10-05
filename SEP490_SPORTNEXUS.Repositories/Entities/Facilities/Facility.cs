using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;
using SEP490_SPORTNEXUS_BE.Repositories.Abstraction;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class Facility : Entity<Guid>, IAuditable
    {
        [Required]
        public Guid OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public Account Owner { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public Point? Location { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Active";

        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? ModifiedAt { get; set; }

        public ICollection<FacilityImage> Images { get; set; } = new List<FacilityImage>();
        public ICollection<FacilityAmenity> FacilityAmenities { get; set; } = new List<FacilityAmenity>();
        public ICollection<Court> Courts { get; set; } = new List<Court>();
        public ICollection<FacilityReview> Reviews { get; set; } = new List<FacilityReview>();
    }
}
