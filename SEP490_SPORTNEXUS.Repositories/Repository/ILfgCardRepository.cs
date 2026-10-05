using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface ILfgCardRepository : IGenericRepository<LfgCard>
    {
        Task<IEnumerable<LfgCard>> GetActiveLfgCardsAsync();
        Task<LfgCard?> GetLfgCardForUpdateAsync(Guid cardId);
    }
}