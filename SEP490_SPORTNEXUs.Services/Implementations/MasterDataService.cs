using SEP490_SPORTNEXUS_BE.Repositories.Repository;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class MasterDataService : IMasterDataService
    {
        private readonly IMasterDataRepository _repo;
        public MasterDataService(IMasterDataRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<IEnumerable<SportCategoryResponse>>> GetAllSportCategoriesAsync()
        {
            var data = await _repo.GetAllSportCategoriesAsync();
            var mapped = data.Select(x => new SportCategoryResponse { Id = x.Id, Name = x.Name, RulesDescription = x.RulesDescription });
            return new ApiResponse<IEnumerable<SportCategoryResponse>> { StatusCode = 200, Message = "Success", Data = mapped };
        }

        public async Task<ApiResponse<IEnumerable<AmenityResponse>>> GetAllAmenitiesAsync()
        {
            var data = await _repo.GetAllAmenitiesAsync();
            var mapped = data.Select(x => new AmenityResponse { Id = x.Id, Name = x.Name, IconCode = x.IconCode });
            return new ApiResponse<IEnumerable<AmenityResponse>> { StatusCode = 200, Message = "Success", Data = mapped };
        }
    }
}
