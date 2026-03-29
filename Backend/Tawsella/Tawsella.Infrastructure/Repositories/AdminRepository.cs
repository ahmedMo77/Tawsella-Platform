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
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Admin?> GetAdminWithUserAsync(string adminId, CancellationToken ct = default)
        {
            return await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == adminId, ct);
        }

        public async Task<IReadOnlyList<Admin>> GetSuperAdminsAsync(CancellationToken ct = default)
        {
            return await _context.Admins
                .Where(a => a.IsSuperAdmin)
                .Include(a => a.User)
                .ToListAsync(ct);
        }
    }
}