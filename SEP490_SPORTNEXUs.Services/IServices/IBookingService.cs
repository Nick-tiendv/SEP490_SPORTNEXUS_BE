using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IBookingService
    {
        Task<ApiResponse<object?>> CreateBookingAsync(Guid hostId, CreateBookingRequest request);
        Task<ApiResponse<object?>> JoinBookingAsync(Guid userId, Guid bookingId);
        Task<ApiResponse<string?>> GetDynamicQrTicketAsync(Guid userId, Guid bookingId);
    }
}