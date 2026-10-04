using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using System;
using System.Threading.Tasks;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class WalletRepository : GenericRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context) {}

        public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }
        
        public async Task<Wallet> GetOrCreateWalletAsync(Guid userId)
        {
            var wallet = await GetWalletByUserIdAsync(userId);
            if (wallet == null) {
                wallet = new Wallet { Id = Guid.NewGuid(), UserId = userId, Balance = 0, FrozenBalance = 0 };
                await _context.Wallets.AddAsync(wallet);
                await _context.SaveChangesAsync();
            }
            return wallet;
        }

        public async Task UpdateWalletBalanceAsync(Guid walletId, decimal amount, bool isFreeze)
        {
            // Simple update, normally handled inside service transaction
            var w = await GetByIdAsync(walletId);
            if (w == null) return;
            if (isFreeze) {
                w.Balance -= amount;
                w.FrozenBalance += amount;
            } else {
                w.Balance += amount;
            }
            _context.Wallets.Update(w);
            await _context.SaveChangesAsync();
        }
    }
}