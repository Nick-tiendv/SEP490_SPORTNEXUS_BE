using System;
using System.Collections.Generic;

namespace SEP490_SPORTNEXUS_BE.Services.RequestModel
{
    public class CreateBookingRequest
    {
        public Guid CourtSlotId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsSplitPayment { get; set; }
        public List<Guid>? ParticipantIds { get; set; }
    }
}