using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class SubMenuMasterRepository : ISubMenuMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public SubMenuMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<SubMenuMaster>> GetAllAsync()
        {
            return await _context.SubMenuMasters.AsNoTracking().OrderBy(s => s.SubMenuId).ToListAsync();
        }

        public async Task<SubMenuMaster?> GetByIdAsync(int subMenuId)
        {
            return await _context.SubMenuMasters.AsNoTracking().FirstOrDefaultAsync(s => s.SubMenuId == subMenuId);
        }

        public async Task<SubMenuMaster> AddAsync(SubMenuMaster subMenuMaster)
        {
            _context.SubMenuMasters.Add(subMenuMaster);
            await _context.SaveChangesAsync();
            return subMenuMaster;
        }

        public async Task<bool> UpdateAsync(SubMenuMaster subMenuMaster)
        {
            var existing = await _context.SubMenuMasters.FirstOrDefaultAsync(s => s.SubMenuId == subMenuMaster.SubMenuId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(subMenuMaster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int subMenuId)
        {
            var existing = await _context.SubMenuMasters.FirstOrDefaultAsync(s => s.SubMenuId == subMenuId);
            if (existing == null)
            {
                return false;
            }

            _context.SubMenuMasters.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubMenuNameExistsAsync(string subMenuName, int menuId, int roleId, int menuCategoryId, int? excludeSubMenuId = null)
        {
            return await _context.SubMenuMasters.AnyAsync(s =>
                s.SubMenuName == subMenuName && s.MenuId == menuId && s.RoleId == roleId
                && s.MenuCategoryId == menuCategoryId && s.SubMenuId != excludeSubMenuId);
        }
    }
}
