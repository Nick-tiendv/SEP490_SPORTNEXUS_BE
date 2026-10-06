using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.System;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        public PaymentService(ApplicationDbContext context) { _context = context; }

        public async Task<ApiResponse<object?>> ProcessWebhookAsync(string gateway, string signature, string rawPayload)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                // Mock Signature Verification
                if (string.IsNullOrEmpty(signature)) return new ApiResponse<object?> { StatusCode = 401, Message = "Invalid Signature" };
                
                // Giả lập Webhook gọi tới báo thành công cho 1 TransactionId ngẫu nhiên (hoặc parse từ rawPayload)
                // Vì không có cục raw thật, ta chỉ log nó lại để đối soát
                var log = new PaymentGatewayLog { Id = Guid.NewGuid(), TransactionId = Guid.Empty, Gateway = gateway, RawPayload = rawPayload };
                _context.PaymentGatewayLogs.Add(log);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return new ApiResponse<object?> { StatusCode = 200, Message = "Webhook processed" };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        public NotificationService(ApplicationDbContext context) { _context = context; }

        public async Task SendAndLogNotificationAsync(Guid userId, string title, string body, string type)
        {
            var notif = new NotificationLog { Id = Guid.NewGuid(), UserId = userId, Title = title, Body = body, Type = type };
            _context.NotificationLogs.Add(notif);
            await _context.SaveChangesAsync();
            // TODO: Call SignalR Hub here
        }

        public async Task<ApiResponse<object?>> GetUserNotificationsAsync(Guid userId)
        {
            var data = await _context.NotificationLogs.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();
            if (!data.Any()) return new ApiResponse<object?> { StatusCode = 200, Message = "Bạn chưa có thông báo nào", Data = data };
            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = data };
        }

        public async Task<ApiResponse<object?>> MarkAsReadAsync(Guid notificationId)
        {
            var n = await _context.NotificationLogs.FindAsync(notificationId);
            if (n != null) { n.IsRead = true; _context.NotificationLogs.Update(n); await _context.SaveChangesAsync(); }
            return new ApiResponse<object?> { StatusCode = 200, Message = "Read" };
        }
    }
}