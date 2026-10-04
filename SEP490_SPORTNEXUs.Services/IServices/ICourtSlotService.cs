using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface ICourtSlotService
    {
        Task<ApiResponse<object?>> GenerateCourtSlotsAsync(Guid courtId, Guid ownerId, GenerateCourtSlotsRequest request);
        Task<ApiResponse<IEnumerable<CourtSlotResponse>>> GetTimeGridAsync(Guid courtId, DateTime date);
        Task<ApiResponse<CourtSlotResponse?>> UpdateSlotStatusAsync(Guid slotId, Guid ownerId, UpdateSlotStatusRequest request);
    }
}
