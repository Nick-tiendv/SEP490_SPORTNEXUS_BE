using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(Account account);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
