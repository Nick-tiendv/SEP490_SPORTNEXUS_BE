using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface ITournamentRepository : IGenericRepository<Tournament>
    {
        Task<IEnumerable<Tournament>> GetTournamentHierarchyAsync(Guid parentId);
    }

    public interface IBracketMatchRepository : IGenericRepository<BracketMatch>
    {
        Task<IEnumerable<BracketMatch>> GetFullBracketAsync(Guid tournamentId);
        Task<BracketMatch?> GetMatchByIndexAsync(Guid tournamentId, int index);
    }
}