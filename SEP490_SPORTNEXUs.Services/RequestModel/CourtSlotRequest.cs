using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using System;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public class GenerateCourtSlotsRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateSlotStatusRequest
    {
        public CourtSlotStatus Status { get; set; }
        public decimal? Price { get; set; }
    }
}
