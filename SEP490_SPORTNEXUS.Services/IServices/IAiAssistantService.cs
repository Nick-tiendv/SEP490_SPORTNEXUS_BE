using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IAiAssistantService
    {
        Task<ApiResponse<object?>> CreateSessionAsync(Guid userId);
        Task<ApiResponse<object?>> GetSessionHistoryAsync(Guid userId);
        Task<ApiResponse<object?>> GetMessagesAsync(Guid sessionId);
        Task<ApiResponse<object?>> ProcessChatAsync(Guid userId, Guid sessionId, string userMessage);
    }
}