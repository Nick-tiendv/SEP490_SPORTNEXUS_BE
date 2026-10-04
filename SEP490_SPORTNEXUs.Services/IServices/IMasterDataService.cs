using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface IMasterDataService
    {
        Task<ApiResponse<IEnumerable<SportCategoryResponse>>> GetAllSportCategoriesAsync();
        Task<ApiResponse<IEnumerable<AmenityResponse>>> GetAllAmenitiesAsync();
    }
}
