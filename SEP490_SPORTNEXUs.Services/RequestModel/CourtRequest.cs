using System;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public class CreateCourtRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public decimal DefaultPrice { get; set; }
    }
    
    public class UpdateCourtRequest
    {
        public Guid? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public decimal? DefaultPrice { get; set; }
    }
}
