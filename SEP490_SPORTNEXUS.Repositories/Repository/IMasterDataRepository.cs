using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface IMasterDataRepository
    {
        Task<IEnumerable<SportCategory>> GetAllSportCategoriesAsync();
        Task<IEnumerable<Amenity>> GetAllAmenitiesAsync();
    }
}
