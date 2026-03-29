using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    public interface IWalletRepository : IAsyncRepository<Wallet>
    {
        Task<Wallet?> GetByCourierIdAsync(string courierId, CancellationToken ct = default);
        Task UpdateBalanceAsync(string courierId, decimal amount, CancellationToken ct = default);
        Task<IReadOnlyList<WalletTransaction>> GetTransactionsByWalletIdAsync(string walletId, CancellationToken ct = default);
    }
}
