using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class CourtSlotRepository : GenericRepository<CourtSlot>, ICourtSlotRepository
    {
        public CourtSlotRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CourtSlot>> GetSlotsByDateRangeAsync(Guid courtId, DateTimeOffset fromDate, DateTimeOffset toDate)
        {
            return await _context.CourtSlots
                .Where(s => s.CourtId == courtId && s.StartTime >= fromDate && s.StartTime <= toDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        
        public async Task<CourtSlot?> GetSlotForUpdateAsync(Guid slotId)
        {
            return await _context.CourtSlots
                .FromSqlInterpolated($"SELECT * FROM \"CourtSlots\" WHERE \"Id\" = {slotId} FOR UPDATE")
                .FirstOrDefaultAsync();
        }
        public async Task BulkInsertSlotsAsync(List<CourtSlot> slots)
        {
            await _context.CourtSlots.AddRangeAsync(slots);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckOverlappingSlotsAsync(Guid courtId, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            return await _context.CourtSlots.AnyAsync(s => 
                s.CourtId == courtId && 
                s.StartTime < endTime && s.EndTime > startTime);
        }
    }
}
