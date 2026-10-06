using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;

namespace SEP490_SPORTNEXUS_BE.Services.ResponseModel
{
    public class CourtSlotResponse
    {
        public Guid Id { get; set; }
        public Guid CourtId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
    }
}
