using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context) {}

        public async Task<IEnumerable<Booking>> GetMyBookingsAsync(Guid userId)
        {
            return await _context.Bookings
                .Include(b => b.CourtSlot)
                    .ThenInclude(cs => cs.Court)
                        .ThenInclude(c => c.Facility)
                .Include(b => b.Participants)
                .Where(b => b.HostId == userId || b.Participants.Any(p => p.UserId == userId))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
}