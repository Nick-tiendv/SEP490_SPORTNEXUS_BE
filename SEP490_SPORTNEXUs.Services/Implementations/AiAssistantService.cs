using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.AiAssistant;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class AiAssistantService : IAiAssistantService
    {
        private readonly ApplicationDbContext _context;

        public AiAssistantService(ApplicationDbContext context) { _context = context; }

        public async Task<ApiResponse<object?>> CreateSessionAsync(Guid userId)
        {
            var session = new AiChatSession { Id = Guid.NewGuid(), UserId = userId, SessionTitle = "Tìm kiếm sân tự động" };
            _context.AiChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return new ApiResponse<object?> { StatusCode = 201, Message = "Tạo phiên chat thành công", Data = new { session.Id } };
        }

        public async Task<ApiResponse<object?>> GetSessionHistoryAsync(Guid userId)
        {
            var sessions = await _context.AiChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new { s.Id, s.SessionTitle, s.CreatedAt })
                .ToListAsync();
            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = sessions };
        }

        public async Task<ApiResponse<object?>> GetMessagesAsync(Guid sessionId)
        {
            var messages = await _context.AiPromptLogs
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new { m.Id, m.Sender, m.Message, m.IntentExtracted, m.CreatedAt })
                .ToListAsync();
            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = messages };
        }

        public async Task<ApiResponse<object?>> ProcessChatAsync(Guid userId, Guid sessionId, string userMessage)
        {
            // Verify session belongs to user
            var session = await _context.AiChatSessions.FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);
            if (session == null) return new ApiResponse<object?> { StatusCode = 404, Message = "Session not found." };

            // 1. Log User Message
            var userLog = new AiPromptLog { Id = Guid.NewGuid(), SessionId = sessionId, Sender = "USER", Message = userMessage };
            _context.AiPromptLogs.Add(userLog);
            await _context.SaveChangesAsync(); // Save immediately in case of error

            // 2. MOCK LLM: Trích xuất Intent
            var intentMock = new { Sport = "Cầu lông", Location = "Quận 1", Date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"), Time = "19:00" };
            string intentJson = JsonSerializer.Serialize(intentMock);

            // 3. Thực thi nghiệp vụ (MOCK Query)
            // Normal flow: We would inject IFacilityService here and search by Location/Sport.
            // For now, we simulate finding 2 courts.
            string botResponse = $"Dạ, em hiểu anh/chị đang muốn tìm sân {intentMock.Sport} ở {intentMock.Location} vào lúc {intentMock.Time} ngày {intentMock.Date}. Dưới đây là 2 sân còn trống:\n\n1. Sân Vio Quận 1 (Trống 19:00 - 50k)\n2. Sân Galaxy Quận 1 (Trống 19:00 - 55k)\n\nAnh/chị muốn đặt sân nào ạ?";

            // 4. Log Bot Message
            var botLog = new AiPromptLog { Id = Guid.NewGuid(), SessionId = sessionId, Sender = "BOT", Message = botResponse, IntentExtracted = intentJson };
            _context.AiPromptLogs.Add(botLog);
            
            // Update Session Title to match intent briefly
            session.SessionTitle = $"Tìm {intentMock.Sport} {intentMock.Location}";
            _context.AiChatSessions.Update(session);

            await _context.SaveChangesAsync();

            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = new { botLog.Message, botLog.IntentExtracted } };
        }
    }
}