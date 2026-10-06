using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IFacilityService
    {
        Task<ApiResponse<IEnumerable<FacilityResponse>>> SearchFacilitiesAsync(double? lat, double? lng, double? radius, Guid? categoryId, Guid? amenityId, string? status);
        Task<ApiResponse<FacilityResponse?>> GetFacilityDetailsAsync(Guid id);
        Task<ApiResponse<FacilityResponse?>> CreateFacilityAsync(Guid ownerId, CreateFacilityRequest request);
        Task<ApiResponse<FacilityResponse?>> UpdateFacilityAsync(Guid facilityId, Guid ownerId, UpdateFacilityRequest request);
    }
}
