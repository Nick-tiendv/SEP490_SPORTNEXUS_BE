using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetMyBookingsAsync(Guid userId);
    }
}