using System;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class CourtResponse
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal DefaultPrice { get; set; }
    }
}
