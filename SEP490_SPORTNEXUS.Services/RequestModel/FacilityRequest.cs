using System;
using System.Collections.Generic;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public class CreateFacilityRequest
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<Guid> AmenityIds { get; set; } = new List<Guid>();
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
    
    public class UpdateFacilityRequest
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Status { get; set; }
        public List<Guid>? AmenityIds { get; set; }
        public List<string>? ImageUrls { get; set; }
    }
}
