using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class LfgCardRepository : GenericRepository<LfgCard>, ILfgCardRepository
    {
        public LfgCardRepository(ApplicationDbContext context) : base(context) {}

        public async Task<IEnumerable<LfgCard>> GetActiveLfgCardsAsync()
        {
            return await _context.LfgCards
                .Include(c => c.Booking)
                    .ThenInclude(b => b.CourtSlot)
                        .ThenInclude(cs => cs.Court)
                .Where(c => c.Status == Enums.LfgCardStatus.PENDING)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<LfgCard?> GetLfgCardForUpdateAsync(Guid cardId)
        {
            return await _context.LfgCards
                .FromSqlInterpolated($"SELECT * FROM \"LfgCards\" WHERE \"Id\" = {cardId} FOR UPDATE")
                .FirstOrDefaultAsync();
        }
    }
}