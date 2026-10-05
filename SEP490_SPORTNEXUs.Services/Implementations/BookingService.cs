using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using SEP490_SPORTNEXUS_BE.Repositories.Repository;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourtSlotRepository _slotRepo;
        private readonly IWalletRepository _walletRepo;

        public BookingService(ApplicationDbContext context, ICourtSlotRepository slotRepo, IWalletRepository walletRepo)
        {
            _context = context;
            _slotRepo = slotRepo;
            _walletRepo = walletRepo;
        }

        public async Task<ApiResponse<object?>> CreateBookingAsync(Guid hostId, CreateBookingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Pessimistic Lock
                var slot = await _slotRepo.GetSlotForUpdateAsync(request.CourtSlotId);
                if (slot == null || slot.Status != CourtSlotStatus.Available)
                {
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Sân đã có người đặt hoặc đang bảo trì", Data = null };
                }

                // 2. Check Wallet
                var wallet = await _walletRepo.GetOrCreateWalletAsync(hostId);
                
                // GIẢI PHÁP 3: Host phải thanh toán 100% tiền sân ngay lúc đặt
                if (wallet.Balance < request.TotalAmount)
                {
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Số dư không đủ để thanh toán toàn bộ ca sân", Data = null };
                }

                // 3. Trừ thẳng tiền của Host (Không dùng FrozenBalance nữa vì chốt luôn)
                wallet.Balance -= request.TotalAmount;
                _context.Wallets.Update(wallet);

                // 4. Log Transaction
                _context.Transactions.Add(new Transaction {
                    Id = Guid.NewGuid(), WalletId = wallet.Id, Type = TransactionType.PAYOUT,
                    Amount = request.TotalAmount, Status = "SUCCESS"
                });

                // 5. Update Slot Status -> BOOKED ngay lập tức
                slot.Status = CourtSlotStatus.Booked;
                _context.CourtSlots.Update(slot);

                // 6. Create Booking
                var booking = new Booking
                {
                    Id = Guid.NewGuid(), HostId = hostId, CourtSlotId = slot.Id,
                    TotalAmount = request.TotalAmount, Status = BookingStatus.PAID
                };
                _context.Bookings.Add(booking);

                // 7. Xử lý logic chia tiền (Split-Payment)
                int totalPeople = 1 + (request.ParticipantIds?.Count ?? 0);
                decimal amountPerPerson = request.TotalAmount / totalPeople;

                // Lưu Host (Đã trả 100% cho hệ thống, nhưng về mặt sổ sách thì chỉ chịu 1 phần)
                _context.BookingParticipants.Add(new BookingParticipant {
                    Id = Guid.NewGuid(), BookingId = booking.Id, UserId = hostId,
                    AmountContributed = amountPerPerson, PaymentStatus = PaymentStatus.PAID
                });

                // Lưu Bạn bè (Đang nợ tiền Host)
                if (request.IsSplitPayment && request.ParticipantIds != null)
                {
                    foreach (var friendId in request.ParticipantIds)
                    {
                        _context.BookingParticipants.Add(new BookingParticipant {
                            Id = Guid.NewGuid(), BookingId = booking.Id, UserId = friendId,
                            AmountContributed = amountPerPerson, PaymentStatus = PaymentStatus.PENDING
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse<object?> { StatusCode = 201, Message = "Đặt sân thành công", Data = new { booking.Id } };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<object?> { StatusCode = 500, Message = "Lỗi hệ thống: " + ex.Message, Data = null };
            }
        }

        public async Task<ApiResponse<object?>> JoinBookingAsync(Guid userId, Guid bookingId)
        {
            // API để bạn bè trả tiền lại cho Host
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                var participant = _context.BookingParticipants.FirstOrDefault(p => p.BookingId == bookingId && p.UserId == userId);
                if (participant == null || participant.PaymentStatus == PaymentStatus.PAID) 
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Không tìm thấy yêu cầu góp tiền hoặc bạn đã trả rồi", Data = null };
                
                var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
                var friendWallet = await _walletRepo.GetOrCreateWalletAsync(userId);
                var hostWallet = await _walletRepo.GetOrCreateWalletAsync(booking!.HostId);

                if (friendWallet.Balance < participant.AmountContributed)
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Số dư của bạn không đủ để góp tiền", Data = null };

                // Trừ tiền bạn bè
                friendWallet.Balance -= participant.AmountContributed;
                _context.Transactions.Add(new Transaction {
                    Id = Guid.NewGuid(), WalletId = friendWallet.Id, Type = TransactionType.PAYOUT, Amount = participant.AmountContributed, Status = "SUCCESS", ReferenceId = booking.Id
                });

                // Chuyển thẳng cho Host
                hostWallet.Balance += participant.AmountContributed;
                _context.Transactions.Add(new Transaction {
                    Id = Guid.NewGuid(), WalletId = hostWallet.Id, Type = TransactionType.DEPOSIT, Amount = participant.AmountContributed, Status = "SUCCESS", ReferenceId = booking.Id
                });

                // Cập nhật trạng thái
                participant.PaymentStatus = PaymentStatus.PAID;

                _context.Wallets.Update(friendWallet);
                _context.Wallets.Update(hostWallet);
                _context.BookingParticipants.Update(participant);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse<object?> { StatusCode = 200, Message = "Góp tiền thành công", Data = null };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse<object?> { StatusCode = 500, Message = "Lỗi hệ thống: " + ex.Message, Data = null };
            }
        }


        public async Task<ApiResponse<object?>> CancelBookingAsync(Guid bookingId, Guid userId, string reason)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var booking = await _context.Bookings.Include(b => b.CourtSlot).FirstOrDefaultAsync(b => b.Id == bookingId);
                if (booking == null) return new ApiResponse<object?> { StatusCode = 404, Message = "Booking not found" };

                var timeDiff = booking.CourtSlot!.StartTime - DateTime.UtcNow;
                decimal refundPct = 0;
                if (timeDiff.TotalHours >= 24) refundPct = 1.0m;
                else if (timeDiff.TotalHours >= 12) refundPct = 0.5m;
                else refundPct = 0m;

                decimal refundAmount = booking.TotalAmount * refundPct;
                decimal penaltyAmount = booking.TotalAmount - refundAmount;

                var cancelLog = new SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings.BookingCancellation {
                    Id = Guid.NewGuid(), BookingId = bookingId, CancelledBy = userId, Reason = reason,
                    RefundAmount = refundAmount, PenaltyAmount = penaltyAmount
                };
                _context.Set<SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings.BookingCancellation>().Add(cancelLog);

                // Refund to Wallet
                var wallet = await _walletRepo.GetOrCreateWalletAsync(booking.HostId);
                wallet.Balance += refundAmount;
                _context.Wallets.Update(wallet);

                booking.Status = BookingStatus.CANCELLED;
                booking.CourtSlot.Status = CourtSlotStatus.Available;

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = $"Hủy đơn thành công. Hoàn tiền: {refundAmount}, Phạt: {penaltyAmount}" };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<string?>> GetDynamicQrTicketAsync(Guid userId, Guid bookingId)
        {
            long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() / 30; // 30 sec window
            string dynamicQr = $"TICKET_{bookingId}_{userId}_{unixTime}";
            return new ApiResponse<string?> { StatusCode = 200, Message = "Success", Data = dynamicQr };
        }
    }
}
