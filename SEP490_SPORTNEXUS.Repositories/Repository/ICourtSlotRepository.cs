using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface ICourtSlotRepository : IGenericRepository<CourtSlot>
    {
        Task<IEnumerable<CourtSlot>> GetSlotsByDateRangeAsync(Guid courtId, DateTimeOffset fromDate, DateTimeOffset toDate);
        Task<CourtSlot?> GetSlotForUpdateAsync(Guid slotId);
        Task BulkInsertSlotsAsync(List<CourtSlot> slots);
        Task<bool> CheckOverlappingSlotsAsync(Guid courtId, DateTimeOffset startTime, DateTimeOffset endTime);
    }
}
