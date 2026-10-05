using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
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
    public class TournamentService : ITournamentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITournamentRepository _tourRepo;
        private readonly IBracketMatchRepository _bracketRepo;
        private readonly IWalletRepository _walletRepo;

        public TournamentService(ApplicationDbContext context, ITournamentRepository tourRepo, IBracketMatchRepository bracketRepo, IWalletRepository walletRepo)
        {
            _context = context; _tourRepo = tourRepo; _bracketRepo = bracketRepo; _walletRepo = walletRepo;
        }


        public async Task<ApiResponse<object?>> CreateTournamentAsync(CreateTournamentRequest request)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var parentId = Guid.NewGuid();
                var parent = new Tournament {
                    Id = parentId, FacilityId = request.FacilityId, CategoryId = request.CategoryId,
                    Name = request.Name, CategoryType = TournamentCategoryType.SINGLES, // Dummy for parent
                    MaxParticipants = 0, Status = TournamentStatus.OPEN
                };
                _context.Tournaments.Add(parent);

                foreach (var childDto in request.Children)
                {
                    var childId = Guid.NewGuid();
                    var child = new Tournament {
                        Id = childId, ParentTournamentId = parentId, FacilityId = request.FacilityId, CategoryId = request.CategoryId,
                        Name = childDto.Name, CategoryType = childDto.CategoryType, MaxParticipants = childDto.MaxParticipants,
                        EntryFee = childDto.EntryFee, Status = TournamentStatus.OPEN
                    };
                    _context.Tournaments.Add(child);

                    foreach (var p in childDto.Prizes)
                    {
                        _context.TournamentPrizes.Add(new TournamentPrize {
                            Id = Guid.NewGuid(), TournamentId = childId, Position = p.Position, RewardAmount = p.RewardAmount
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 201, Message = "Tạo Giải đấu thành công", Data = new { parentId } };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<object?>> GetTournamentsAsync()
        {
            var tours = await _context.Tournaments
                .Where(t => !t.ParentTournamentId.HasValue) // Lấy giải mẹ
                .Select(t => new { t.Id, t.Name, t.Status })
                .ToListAsync();
            if (!tours.Any()) return new ApiResponse<object?> { StatusCode = 200, Message = "Hiện không có giải đấu nào đang mở", Data = tours };
            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = tours };
        }

        public async Task<ApiResponse<object?>> GetBracketAsync(Guid tournamentId)
        {
            var bracket = await _bracketRepo.GetFullBracketAsync(tournamentId);
            var result = bracket.Select(m => new {
                m.Id, m.MatchIndex, m.RoundName, m.Status,
                P1 = m.Participant1?.Team?.Name ?? m.Participant1?.User?.FullName ?? "TBD",
                P2 = m.Participant2?.Team?.Name ?? m.Participant2?.User?.FullName ?? "TBD",
                Scores = m.Scores.Select(s => new { s.SetNumber, s.ScoreP1, s.ScoreP2 })
            });
            return new ApiResponse<object?> { StatusCode = 200, Message = "Success", Data = result };
        }

        public async Task<ApiResponse<object?>> RegisterTournamentAsync(Guid userId, Guid tournamentId, Guid? teamId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var tour = await _tourRepo.GetByIdAsync(tournamentId);
                if (tour == null || tour.Status != TournamentStatus.OPEN) return new ApiResponse<object?> { StatusCode = 400, Message = "Giải đấu không tồn tại hoặc đã đóng cửa." };
                
                if (tour.CategoryType == TournamentCategoryType.RANDOM_DOUBLES && teamId.HasValue)
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Giải đấu ghép ngẫu nhiên bắt buộc phải đăng ký cá nhân!" };

                var wallet = await _walletRepo.GetOrCreateWalletAsync(userId);
                if (wallet.Balance < tour.EntryFee) return new ApiResponse<object?> { StatusCode = 400, Message = "Số dư không đủ." };

                wallet.Balance -= tour.EntryFee;
                wallet.FrozenBalance += tour.EntryFee;
                _context.Wallets.Update(wallet);

                var participant = new TournamentParticipant { Id = Guid.NewGuid(), TournamentId = tournamentId, UserId = userId, TeamId = teamId, PaymentStatus = PaymentStatus.PAID };
                _context.TournamentParticipants.Add(participant);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = "Đăng ký thành công" };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<object?>> ExecuteRandomPairingAsync(Guid tournamentId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var tour = await _tourRepo.GetByIdAsync(tournamentId);
                if (tour == null || tour.CategoryType != TournamentCategoryType.RANDOM_DOUBLES) 
                    return new ApiResponse<object?> { StatusCode = 400, Message = "Giải này không phải random đôi." };

                var players = await _context.TournamentParticipants.Where(p => p.TournamentId == tournamentId && !p.TeamId.HasValue && !p.IsDeleted).ToListAsync();
                if (players.Count % 2 != 0) return new ApiResponse<object?> { StatusCode = 400, Message = "Số lượng người chơi lẻ, không thể ghép đôi." };

                // Shuffle
                var random = new Random();
                players = players.OrderBy(x => random.Next()).ToList();

                for (int i = 0; i < players.Count; i += 2)
                {
                    var p1 = players[i]; var p2 = players[i+1];
                    var team = new Team { Id = Guid.NewGuid(), Name = $"Cặp bốc thăm {i/2 + 1}", CreatedById = p1.UserId!.Value };
                    _context.Teams.Add(team);

                    var newP = new TournamentParticipant { Id = Guid.NewGuid(), TournamentId = tournamentId, TeamId = team.Id, PaymentStatus = PaymentStatus.PAID };
                    _context.TournamentParticipants.Add(newP);

                    p1.IsDeleted = true; // Mark old solo entries as deleted / merged
                    p2.IsDeleted = true;
                }
                
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = "Bốc thăm ghép đôi hoàn tất." };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<object?>> GenerateBracketAsync(Guid tournamentId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var participants = await _context.TournamentParticipants.Where(p => p.TournamentId == tournamentId && !p.IsDeleted).ToListAsync();
                if (participants.Count < 2) return new ApiResponse<object?> { StatusCode = 400, Message = "Không đủ số lượng để xếp lịch." };

                // Rất cơ bản: Dùng logic Math-based Binary Tree. Số đội thường là lũy thừa của 2 (2, 4, 8, 16).
                // Giả định chuẩn lũy thừa của 2 để minh họa. Final = 1. Semis = 2, 3. Quarters = 4,5,6,7.
                int numTeams = participants.Count; 
                int totalMatches = numTeams - 1;
                
                var matches = new List<BracketMatch>();
                // Create empty matches for future rounds
                for(int i = 1; i <= totalMatches - (numTeams / 2); i++) {
                    matches.Add(new BracketMatch { Id = Guid.NewGuid(), TournamentId = tournamentId, MatchIndex = i, RoundName = $"Round {i}", Status = "PENDING" });
                }

                // Create first round matches and assign teams
                int firstRoundStartIndex = totalMatches - (numTeams / 2) + 1;
                int pIndex = 0;
                for (int i = firstRoundStartIndex; i <= totalMatches; i++) {
                    matches.Add(new BracketMatch {
                        Id = Guid.NewGuid(), TournamentId = tournamentId, MatchIndex = i, RoundName = "First Round",
                        Participant1Id = participants[pIndex].Id, Participant2Id = participants[pIndex+1].Id, Status = "SCHEDULED"
                    });
                    pIndex += 2;
                }

                _context.BracketMatches.AddRange(matches);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = $"Sinh sơ đồ thành công với {matches.Count} trận đấu." };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<object?>> UpdateScoreAndAdvanceAsync(Guid matchId, int scoreP1, int scoreP2, Guid winnerParticipantId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try {
                var match = await _context.BracketMatches.FirstOrDefaultAsync(m => m.Id == matchId);
                if (match == null) return new ApiResponse<object?> { StatusCode = 404, Message = "Trận đấu không tồn tại." };

                match.WinnerId = winnerParticipantId;
                match.Status = "FINISHED";
                _context.MatchScoreDetails.Add(new MatchScoreDetail { Id = Guid.NewGuid(), BracketMatchId = matchId, SetNumber = 1, ScoreP1 = scoreP1, ScoreP2 = scoreP2 });

                // Advancement Logic O(1)
                if (match.MatchIndex > 1) {
                    int nextMatchIndex = match.MatchIndex / 2; // Floor division
                    var nextMatch = await _bracketRepo.GetMatchByIndexAsync(match.TournamentId, nextMatchIndex);
                    if (nextMatch != null) {
                        if (match.MatchIndex % 2 == 0) nextMatch.Participant1Id = winnerParticipantId;
                        else nextMatch.Participant2Id = winnerParticipantId;
                        
                        if (nextMatch.Participant1Id.HasValue && nextMatch.Participant2Id.HasValue) nextMatch.Status = "SCHEDULED";
                    }
                } else {
                    // MatchIndex == 1 (Final). Auto Payout logic!
                    var prizes = await _context.TournamentPrizes.Where(p => p.TournamentId == match.TournamentId).ToListAsync();
                    var firstPrize = prizes.FirstOrDefault(p => p.Position == "1st");
                    if (firstPrize != null) {
                        var winner = await _context.TournamentParticipants.FirstOrDefaultAsync(p => p.Id == winnerParticipantId);
                        if (winner != null && winner.UserId.HasValue) {
                            var wWallet = await _walletRepo.GetOrCreateWalletAsync(winner.UserId.Value);
                            wWallet.Balance += firstPrize.RewardAmount;
                            _context.Transactions.Add(new Transaction { Id = Guid.NewGuid(), WalletId = wWallet.Id, Type = TransactionType.PAYOUT, Amount = firstPrize.RewardAmount, Status = "PRIZE" });
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return new ApiResponse<object?> { StatusCode = 200, Message = "Cập nhật kết quả thành công." };
            } catch (Exception ex) {
                await tx.RollbackAsync(); return new ApiResponse<object?> { StatusCode = 500, Message = ex.Message };
            }
        }
    }
}