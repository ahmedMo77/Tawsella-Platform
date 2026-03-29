using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Entities;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    public class WalletRepository : BaseRepository<Wallet>, IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByCourierIdAsync(string courierId, CancellationToken ct = default)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.CourierId == courierId, ct);
        }

        public async Task UpdateBalanceAsync(string courierId, decimal amount, CancellationToken ct = default)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.CourierId == courierId, ct);

            if (wallet != null)
            {
                wallet.Balance += amount;

                if (amount > 0)
                    wallet.TotalEarnings += amount;

                _context.Wallets.Update(wallet);
            }
        }

        public async Task<IReadOnlyList<WalletTransaction>> GetTransactionsByWalletIdAsync(string walletId, CancellationToken ct = default)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
