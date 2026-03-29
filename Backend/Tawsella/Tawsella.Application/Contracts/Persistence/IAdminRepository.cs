using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    public interface IAdminRepository : IAsyncRepository<Admin>
    {
        Task<Admin?> GetAdminWithUserAsync(string adminId, CancellationToken ct = default);

        Task<IReadOnlyList<Admin>> GetSuperAdminsAsync(CancellationToken ct = default);
    }
}
