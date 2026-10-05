using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class MasterDataRepository : IMasterDataRepository
    {
        private readonly ApplicationDbContext _context;
        public MasterDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SportCategory>> GetAllSportCategoriesAsync()
        {
            return await _context.SportCategories.ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetAllAmenitiesAsync()
        {
            return await _context.Amenities.ToListAsync();
        }
    }
}
