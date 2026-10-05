using System;
using System.ComponentModel.DataAnnotations.Schema;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;

namespace SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities
{
    public class FacilityAmenity
    {
        public Guid FacilityId { get; set; }
        [ForeignKey(nameof(FacilityId))]
        public Facility Facility { get; set; } = null!;

        public Guid AmenityId { get; set; }
        [ForeignKey(nameof(AmenityId))]
        public Amenity Amenity { get; set; } = null!;
    }
}
