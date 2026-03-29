using System.Threading;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    public interface ICustomerRepository : IAsyncRepository<Customer>
    {
        Task<Customer?> GetCustomerProfileAsync(string customerId, CancellationToken cancellationToken = default);

        Task<(int totalOrders, decimal totalSpent)> GetCustomerStatisticsAsync(string customerId, CancellationToken cancellationToken = default);
    }
}
