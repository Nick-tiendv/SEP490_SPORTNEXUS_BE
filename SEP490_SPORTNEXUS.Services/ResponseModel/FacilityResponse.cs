using System;
using System.Collections.Generic;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class FacilityResponse
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Status { get; set; } = null!;
        public List<string> Images { get; set; } = new List<string>();
        public List<AmenityResponse> Amenities { get; set; } = new List<AmenityResponse>();
        public List<CourtResponse> Courts { get; set; } = new List<CourtResponse>();
    }
}
