using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class CourtRepository : GenericRepository<Court>, ICourtRepository
    {
        public CourtRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Court>> GetCourtsByFacilityAsync(Guid facilityId)
        {
            return await _context.Courts
                .Include(c => c.Category)
                .Where(c => c.FacilityId == facilityId)
                .ToListAsync();
        }
    }
}
