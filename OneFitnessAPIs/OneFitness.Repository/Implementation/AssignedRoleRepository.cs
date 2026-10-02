using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class AssignedRoleRepository : IAssignedRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignedRoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AssignedRole>> GetAllAsync()
        {
            return await _context.AssignedRoles.AsNoTracking().ToListAsync();
        }

        public async Task<AssignedRole?> GetByUserIdAsync(int userId)
        {
            return await _context.AssignedRoles.AsNoTracking().FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task<AssignedRole> AddAsync(AssignedRole assignedRole)
        {
            _context.AssignedRoles.Add(assignedRole);
            await _context.SaveChangesAsync();
            return assignedRole;
        }

        public async Task<bool> UpdateAsync(AssignedRole assignedRole)
        {
            var existing = await _context.AssignedRoles.FirstOrDefaultAsync(a => a.AssignedRoleId == assignedRole.AssignedRoleId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(assignedRole);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
