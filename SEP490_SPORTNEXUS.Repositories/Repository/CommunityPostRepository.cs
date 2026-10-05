using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class CommunityPostRepository : GenericRepository<CommunityPost>, ICommunityPostRepository
    {
        public CommunityPostRepository(ApplicationDbContext context) : base(context) {}

        public async Task<IEnumerable<CommunityPost>> GetNewsFeedAsync(int page, int pageSize)
        {
            return await _context.CommunityPosts
                .Include(p => p.Author)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}