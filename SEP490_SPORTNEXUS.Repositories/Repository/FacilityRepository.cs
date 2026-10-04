using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class FacilityRepository : GenericRepository<Facility>, IFacilityRepository
    {
        public FacilityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Facility>> GetFacilitiesNearMeAsync(double lat, double lng, double radiusInKm)
        {
            var location = new Point(lng, lat) { SRID = 4326 };
            return await _context.Facilities
                .Where(f => f.Location != null && f.Location.Distance(location) <= radiusInKm * 1000)
                .Include(f => f.Images)
                .ToListAsync();
        }

        public async Task<Facility?> GetFacilityDetailsAsync(Guid id)
        {
            return await _context.Facilities
                .Include(f => f.Images)
                .Include(f => f.FacilityAmenities)
                    .ThenInclude(fa => fa.Amenity)
                .Include(f => f.Courts)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<Facility>> SearchFacilitiesAsync(Guid? categoryId, Guid? amenityId, string? status)
        {
            var query = _context.Facilities
                .Include(f => f.Images)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(f => f.Courts.Any(c => c.CategoryId == categoryId.Value));
            }
            if (amenityId.HasValue)
            {
                query = query.Where(f => f.FacilityAmenities.Any(fa => fa.AmenityId == amenityId.Value));
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(f => f.Status == status);
            }

            return await query.ToListAsync();
        }
    }
}
