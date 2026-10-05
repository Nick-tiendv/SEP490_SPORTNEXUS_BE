using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface ILfgService
    {
        Task<ApiResponse<object?>> CreateLfgCardAsync(Guid hostId, Guid courtSlotId, int slotsNeeded, string? skillTag, decimal totalAmount);
        Task<ApiResponse<object?>> ClaimSlotAsync(Guid userId, Guid cardId);
    }

    public interface ICommunityService
    {
        Task<ApiResponse<object?>> CreatePostAsync(Guid authorId, string content, List<string> mediaUrls);
        Task<ApiResponse<object?>> CommentOnPostAsync(Guid userId, Guid postId, string content);
    }
}