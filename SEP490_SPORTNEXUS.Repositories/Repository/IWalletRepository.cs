using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public interface IWalletRepository : IGenericRepository<Wallet>
    {
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId);
        Task<Wallet> GetOrCreateWalletAsync(Guid userId);
        Task UpdateWalletBalanceAsync(Guid walletId, decimal amount, bool isFreeze);
    }
}