using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEP490_SPORTNEXUS_BE.Repositories.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _db;
        public AccountRepository(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Account>> GetAllAsync() =>
            await _db.Accounts.Include(a => a.Role).ToListAsync();

        public async Task<Account?> GetByIdAsync(Guid id) =>
            await _db.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddAsync(Account account)
        {
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _db.Accounts.Update(account);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Account account)
        {
            _db.Accounts.Remove(account);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _db.Accounts.AnyAsync(a => a.Username == username);
    }
}
