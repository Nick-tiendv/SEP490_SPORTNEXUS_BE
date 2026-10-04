using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface IFacilityRepository : IGenericRepository<Facility>
    {
        Task<IEnumerable<Facility>> GetFacilitiesNearMeAsync(double lat, double lng, double radiusInKm);
        Task<Facility?> GetFacilityDetailsAsync(Guid id);
        Task<IEnumerable<Facility>> SearchFacilitiesAsync(Guid? categoryId, Guid? amenityId, string? status);
    }
}
