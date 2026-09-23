using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse<object?>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request);
    }
}
