using SEP490_SPORTNEXUS_BE.Services.RequestModel;
using SEP490_SPORTNEXUS_BE.Services.ResponseModel;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Services.IServices
{
    public interface ITournamentService
    {
        Task<ApiResponse<object?>> CreateTournamentAsync(CreateTournamentRequest request);
        Task<ApiResponse<object?>> GetTournamentsAsync();
        Task<ApiResponse<object?>> GetBracketAsync(Guid tournamentId);
        
        Task<ApiResponse<object?>> RegisterTournamentAsync(Guid userId, Guid tournamentId, Guid? teamId);
        Task<ApiResponse<object?>> ExecuteRandomPairingAsync(Guid tournamentId);
        Task<ApiResponse<object?>> GenerateBracketAsync(Guid tournamentId);
        Task<ApiResponse<object?>> UpdateScoreAndAdvanceAsync(Guid matchId, int scoreP1, int scoreP2, Guid winnerParticipantId);
    }
}