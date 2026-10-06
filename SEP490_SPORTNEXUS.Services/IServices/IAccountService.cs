using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using static SEP490_SPORTNEXUS_BE.Services.RequestModel.AccountRequest;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IAccountService
    {
        Task<ApiResponse<IEnumerable<AccountReponse>>> GetAllAsync();
        Task<ApiResponse<AccountReponse?>> GetByIdAsync(Guid id);
        Task<ApiResponse<AccountReponse?>> CreateAsync(CreateAccountRequest request);
        Task<ApiResponse<AccountReponse?>> UpdateAsync(Guid id, UpdateAccountRequest request);
        Task<ApiResponse<object?>> DeleteAsync(Guid id);
    }
}
