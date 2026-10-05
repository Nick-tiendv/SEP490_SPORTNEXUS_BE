using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface ICourtService
    {
        Task<ApiResponse<IEnumerable<CourtResponse>>> GetCourtsByFacilityAsync(Guid facilityId);
        Task<ApiResponse<CourtResponse?>> CreateCourtAsync(Guid facilityId, Guid ownerId, CreateCourtRequest request);
        Task<ApiResponse<CourtResponse?>> UpdateCourtAsync(Guid courtId, Guid ownerId, UpdateCourtRequest request);
        Task<ApiResponse<object?>> DeleteCourtAsync(Guid courtId, Guid ownerId);
    }
}
