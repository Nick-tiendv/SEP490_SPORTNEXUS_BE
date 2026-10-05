using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface ICommunityPostRepository : IGenericRepository<CommunityPost>
    {
        Task<IEnumerable<CommunityPost>> GetNewsFeedAsync(int page, int pageSize);
    }
}