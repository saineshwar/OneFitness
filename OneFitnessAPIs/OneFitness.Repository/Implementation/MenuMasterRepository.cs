using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class MenuMasterRepository : IMenuMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<MenuMaster>> GetAllAsync()
        {
            return await _context.MenuMasters.AsNoTracking().OrderByDescending(m => m.MenuId).ToListAsync();
        }

        public async Task<MenuMaster?> GetByIdAsync(int menuId)
        {
            return await _context.MenuMasters.AsNoTracking().FirstOrDefaultAsync(m => m.MenuId == menuId);
        }

        public async Task<MenuMaster> AddAsync(MenuMaster menuMaster)
        {
            _context.MenuMasters.Add(menuMaster);
            await _context.SaveChangesAsync();
            return menuMaster;
        }

        public async Task<bool> UpdateAsync(MenuMaster menuMaster)
        {
            var existing = await _context.MenuMasters.FirstOrDefaultAsync(m => m.MenuId == menuMaster.MenuId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(menuMaster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int menuId)
        {
            var existing = await _context.MenuMasters.FirstOrDefaultAsync(m => m.MenuId == menuId);
            if (existing == null)
            {
                return false;
            }

            _context.MenuMasters.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MenuNameExistsAsync(string menuName, int roleId, int menuCategoryId, int? excludeMenuId = null)
        {
            return await _context.MenuMasters.AnyAsync(m =>
                m.MenuName == menuName && m.RoleId == roleId && m.MenuCategoryId == menuCategoryId && m.MenuId != excludeMenuId);
        }
    }
}
