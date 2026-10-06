using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class CheckInService : ICheckInService
    {
        private readonly ApplicationDbContext _context;

        public CheckInService(ApplicationDbContext context) { _context = context; }

        public async Task<ApiResponse<object?>> ScanQrCodeAsync(Guid facilityId, string qrCode)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                // 1. Check BookingParticipants
                var bp = await _context.BookingParticipants.FirstOrDefaultAsync(x => x.QrCode == qrCode);
                if (bp != null) {
                    if (bp.IsCheckedIn) return new ApiResponse<object?> { StatusCode = 400, Message = "Vé cá nhân này đã được điểm danh!" };
                    bp.IsCheckedIn = true;
                    _context.BookingParticipants.Update(bp);
                    _context.CheckInLogs.Add(new CheckInLog { Id = Guid.NewGuid(), FacilityId = facilityId, BookingParticipantId = bp.Id });
                    await _context.SaveChangesAsync(); await tx.CommitAsync();
                    return new ApiResponse<object?> { StatusCode = 200, Message = "Check-in Booking thành công!" };
                }

                // 2. Check LfgParticipants
                var lp = await _context.LfgParticipants.FirstOrDefaultAsync(x => x.QrCode == qrCode);
                if (lp != null) {
                    if (lp.IsCheckedIn) return new ApiResponse<object?> { StatusCode = 400, Message = "Vé cá nhân này đã được điểm danh!" };
                    lp.IsCheckedIn = true;
                    _context.LfgParticipants.Update(lp);
                    _context.CheckInLogs.Add(new CheckInLog { Id = Guid.NewGuid(), FacilityId = facilityId, LfgParticipantId = lp.Id });
                    await _context.SaveChangesAsync(); await tx.CommitAsync();
                    return new ApiResponse<object?> { StatusCode = 200, Message = "Check-in LFG thành công!" };
                }

                // 3. Check TournamentParticipants
                var tp = await _context.TournamentParticipants.FirstOrDefaultAsync(x => x.QrCode == qrCode);
                if (tp != null) {
                    if (tp.IsCheckedIn) return new ApiResponse<object?> { StatusCode = 400, Message = "Vé cá nhân này đã được điểm danh!" };
                    tp.IsCheckedIn = true;
                    _context.TournamentParticipants.Update(tp);
                    _context.CheckInLogs.Add(new CheckInLog { Id = Guid.NewGuid(), FacilityId = facilityId, TournamentParticipantId = tp.Id });
                    await _context.SaveChangesAsync(); await tx.CommitAsync();
                    return new ApiResponse<object?> { StatusCode = 200, Message = "Check-in Giải đấu thành công!" };
                }

                return new ApiResponse<object?> { StatusCode = 404, Message = "Không tìm thấy mã QR hợp lệ." };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<object?>> RateFairPlayAsync(Guid reviewerId, Guid revieweeId, Guid contextId, int score, string comment)
        {
            // Bảo vệ "Tấm khiên": Bắt buộc cả 2 phải có IsCheckedIn == true ở LFG hoặc Booking đó
            bool hasValidInteraction = await _context.LfgParticipants.AnyAsync(p => p.LfgCardId == contextId && p.UserId == reviewerId && p.IsCheckedIn) 
                                    && await _context.LfgParticipants.AnyAsync(p => p.LfgCardId == contextId && p.UserId == revieweeId && p.IsCheckedIn);
            
            if (!hasValidInteraction) {
                hasValidInteraction = await _context.BookingParticipants.AnyAsync(p => p.BookingId == contextId && p.UserId == reviewerId && p.IsCheckedIn) 
                                   && await _context.BookingParticipants.AnyAsync(p => p.BookingId == contextId && p.UserId == revieweeId && p.IsCheckedIn);
            }

            if (!hasValidInteraction) return new ApiResponse<object?> { StatusCode = 403, Message = "Chỉ những người cùng Check-in tại sân mới được đánh giá thái độ nhau!" };

            var rating = new FairPlayRating { Id = Guid.NewGuid(), ReviewerId = reviewerId, RevieweeId = revieweeId, ContextId = contextId, Score = score, Comment = comment };
            _context.FairPlayRatings.Add(rating);
            
            // Tính lại điểm
            var reviewee = await _context.Accounts.FindAsync(revieweeId);
            if (reviewee != null) {
                var allRatings = await _context.FairPlayRatings.Where(r => r.RevieweeId == revieweeId).Select(r => r.Score).ToListAsync();
                allRatings.Add(score);
                reviewee.FairPlayScore = (decimal)allRatings.Average();
                _context.Accounts.Update(reviewee);
            }

            await _context.SaveChangesAsync();
            return new ApiResponse<object?> { StatusCode = 201, Message = "Đánh giá thành công!" };
        }

        public async Task<ApiResponse<object?>> AddFacilityReviewAsync(Guid userId, Guid facilityId, int rating, string comment)
        {
            // Bảo vệ: User phải từng check-in ít nhất 1 lần tại sân này thông qua CheckInLogs
            bool hasBeenThere = await _context.CheckInLogs.AnyAsync(c => c.FacilityId == facilityId && 
                ( (c.BookingParticipant != null && c.BookingParticipant.UserId == userId) ||
                  (c.LfgParticipant != null && c.LfgParticipant.UserId == userId) ||
                  (c.TournamentParticipant != null && c.TournamentParticipant.UserId == userId) ));
                  
            if (!hasBeenThere) return new ApiResponse<object?> { StatusCode = 403, Message = "Bạn chưa từng check-in tại sân này, không thể Review!" };

            var fr = new FacilityReview { Id = Guid.NewGuid(), FacilityId = facilityId, UserId = userId, Rating = rating, Comment = comment, CreatedAt = DateTimeOffset.UtcNow };
            _context.Set<FacilityReview>().Add(fr);
            await _context.SaveChangesAsync();

            return new ApiResponse<object?> { StatusCode = 201, Message = "Cảm ơn bạn đã đánh giá sân!" };
        }
    }
}