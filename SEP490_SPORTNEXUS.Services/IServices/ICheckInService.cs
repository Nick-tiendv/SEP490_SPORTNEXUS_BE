using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface ICheckInService
    {
        Task<ApiResponse<object?>> ScanQrCodeAsync(Guid facilityId, string qrCode);
        Task<ApiResponse<object?>> RateFairPlayAsync(Guid reviewerId, Guid revieweeId, Guid contextId, int score, string comment);
        Task<ApiResponse<object?>> AddFacilityReviewAsync(Guid userId, Guid facilityId, int rating, string comment);
    }
}