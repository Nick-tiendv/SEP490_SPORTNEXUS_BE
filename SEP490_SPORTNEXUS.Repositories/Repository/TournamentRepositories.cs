using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class TournamentRepository : GenericRepository<Tournament>, ITournamentRepository
    {
        public TournamentRepository(ApplicationDbContext context) : base(context) {}
        public async Task<IEnumerable<Tournament>> GetTournamentHierarchyAsync(Guid parentId)
        {
            return await _context.Tournaments
                .Where(t => t.Id == parentId || t.ParentTournamentId == parentId)
                .ToListAsync();
        }
    }

    public class BracketMatchRepository : GenericRepository<BracketMatch>, IBracketMatchRepository
    {
        public BracketMatchRepository(ApplicationDbContext context) : base(context) {}
        public async Task<IEnumerable<BracketMatch>> GetFullBracketAsync(Guid tournamentId)
        {
            return await _context.BracketMatches
                .Include(m => m.Participant1).ThenInclude(p => p.User)
                .Include(m => m.Participant1).ThenInclude(p => p.Team)
                .Include(m => m.Participant2).ThenInclude(p => p.User)
                .Include(m => m.Participant2).ThenInclude(p => p.Team)
                .Include(m => m.Scores)
                .Where(m => m.TournamentId == tournamentId)
                .OrderByDescending(m => m.MatchIndex)
                .ToListAsync();
        }

        public async Task<BracketMatch?> GetMatchByIndexAsync(Guid tournamentId, int index)
        {
            return await _context.BracketMatches.FirstOrDefaultAsync(m => m.TournamentId == tournamentId && m.MatchIndex == index);
        }
    }
}