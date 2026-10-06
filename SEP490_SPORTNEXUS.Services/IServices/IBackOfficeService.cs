using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IPaymentService
    {
        Task<ApiResponse<object?>> ProcessWebhookAsync(string gateway, string signature, string rawPayload);
    }

    public interface INotificationService
    {
        Task SendAndLogNotificationAsync(Guid userId, string title, string body, string type);
        Task<ApiResponse<object?>> GetUserNotificationsAsync(Guid userId);
        Task<ApiResponse<object?>> MarkAsReadAsync(Guid notificationId);
    }
}