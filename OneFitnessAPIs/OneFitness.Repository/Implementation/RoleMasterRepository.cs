using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class RoleMasterRepository : IRoleMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RoleMaster>> GetAllAsync()
        {
            return await _context.RoleMasters.AsNoTracking().OrderBy(r => r.RoleId).ToListAsync();
        }

        public async Task<IReadOnlyList<RoleMaster>> GetAllActiveAsync(int? excludeRoleId = null)
        {
            return await _context.RoleMasters.AsNoTracking()
                .Where(r => r.Status && r.RoleId != excludeRoleId)
                .OrderBy(r => r.RoleName)
                .ToListAsync();
        }

        public async Task<RoleMaster?> GetByIdAsync(int roleId)
        {
            return await _context.RoleMasters.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<RoleMaster> AddAsync(RoleMaster roleMaster)
        {
            _context.RoleMasters.Add(roleMaster);
            await _context.SaveChangesAsync();
            return roleMaster;
        }

        public async Task<bool> UpdateAsync(RoleMaster roleMaster)
        {
            var existing = await _context.RoleMasters.FirstOrDefaultAsync(r => r.RoleId == roleMaster.RoleId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(roleMaster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int roleId)
        {
            var existing = await _context.RoleMasters.FirstOrDefaultAsync(r => r.RoleId == roleId);
            if (existing == null)
            {
                return false;
            }

            _context.RoleMasters.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, int? excludeRoleId = null)
        {
            return await _context.RoleMasters.AnyAsync(r => r.RoleName == roleName && r.RoleId != excludeRoleId);
        }
    }
}
