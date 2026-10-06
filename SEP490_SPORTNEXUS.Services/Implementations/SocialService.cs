using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using SEP490_SPORTNEXUS_BE.Repositories.Repository;
using SEP490_SPORTNEXUS_BE.Services.IServices;
using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.Implementations
{
    public class LfgService : ILfgService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILfgCardRepository _lfgRepo;
        private readonly IBookingService _bookingService;

        public LfgService(ApplicationDbContext context, ILfgCardRepository lfgRepo, IBookingService bookingService)
        {
            _context = context;
            _lfgRepo = lfgRepo;
            _bookingService = bookingService;
        }

        public async Task<ApiResponse<object?>> CreateLfgCardAsync(Guid hostId, Guid courtSlotId, int slotsNeeded, string? skillTag, decimal totalAmount)
        {
            // The LFG wrapper directly calls CreateBookingAsync first
            var req = new CreateBookingRequest { CourtSlotId = courtSlotId, TotalAmount = totalAmount, IsSplitPayment = false };
            var bookingRes = await _bookingService.CreateBookingAsync(hostId, req);

            if (bookingRes.StatusCode != 201) return bookingRes;

            Guid bookingId = (Guid)((dynamic)bookingRes.Data!).Id;

            var card = new LfgCard { Id = Guid.NewGuid(), BookingId = bookingId, SlotsNeeded = slotsNeeded, SkillLevelTag = skillTag, Status = LfgCardStatus.PENDING };
            _context.LfgCards.Add(card);
            await _context.SaveChangesAsync();

            return new ApiResponse<object?> { StatusCode = 201, Message = "LFG Card Created", Data = new { card.Id } };
        }

        public async Task<ApiResponse<object?>> ClaimSlotAsync(Guid userId, Guid cardId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var card = await _lfgRepo.GetLfgCardForUpdateAsync(cardId);
                if (card == null || card.Status != LfgCardStatus.PENDING) return new ApiResponse<object?> { StatusCode = 400, Message = "LFG is no longer available" };
                
                if (card.SlotsNeeded <= 0) return new ApiResponse<object?> { StatusCode = 400, Message = "LFG is full" };

                var participant = new LfgParticipant { Id = Guid.NewGuid(), LfgCardId = cardId, UserId = userId, Status = LfgParticipantStatus.ACCEPTED };
                _context.LfgParticipants.Add(participant);

                card.SlotsNeeded -= 1;
                if (card.SlotsNeeded == 0) card.Status = LfgCardStatus.FILLED;
                _context.LfgCards.Update(card);
                
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = "Claimed successfully" };
            } catch (Exception ex) {
                await tx.RollbackAsync();
                return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }
    }

    public class CommunityService : ICommunityService
    {
        private readonly ApplicationDbContext _context;

        public CommunityService(ApplicationDbContext context) { _context = context; }

        public async Task<ApiResponse<object?>> CreatePostAsync(Guid authorId, string content, List<string> mediaUrls)
        {
            var post = new CommunityPost { Id = Guid.NewGuid(), AuthorId = authorId, Content = content, MediaUrls = mediaUrls };
            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();
            return new ApiResponse<object?> { StatusCode = 201, Message = "Post created" };
        }

        public async Task<ApiResponse<object?>> CommentOnPostAsync(Guid userId, Guid postId, string content)
        {
            var comment = new PostComment { Id = Guid.NewGuid(), PostId = postId, UserId = userId, Content = content };
            _context.PostComments.Add(comment);
            await _context.SaveChangesAsync();
            return new ApiResponse<object?> { StatusCode = 201, Message = "Comment added" };
        }
    }
}