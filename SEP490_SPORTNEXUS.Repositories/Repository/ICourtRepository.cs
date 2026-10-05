using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface ICourtRepository : IGenericRepository<Court>
    {
        Task<IEnumerable<Court>> GetCourtsByFacilityAsync(Guid facilityId);
    }
}
