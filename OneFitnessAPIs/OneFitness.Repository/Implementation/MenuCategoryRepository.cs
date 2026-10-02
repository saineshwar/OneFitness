using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class MenuCategoryRepository : IMenuCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<MenuCategory>> GetAllAsync()
        {
            return await _context.MenuCategories.AsNoTracking().OrderBy(m => m.MenuCategoryId).ToListAsync();
        }

        public async Task<MenuCategory?> GetByIdAsync(int menuCategoryId)
        {
            return await _context.MenuCategories.AsNoTracking().FirstOrDefaultAsync(m => m.MenuCategoryId == menuCategoryId);
        }

        public async Task<MenuCategory> AddAsync(MenuCategory menuCategory)
        {
            _context.MenuCategories.Add(menuCategory);
            await _context.SaveChangesAsync();
            return menuCategory;
        }

        public async Task<bool> UpdateAsync(MenuCategory menuCategory)
        {
            var existing = await _context.MenuCategories.FirstOrDefaultAsync(m => m.MenuCategoryId == menuCategory.MenuCategoryId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(menuCategory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int menuCategoryId)
        {
            var existing = await _context.MenuCategories.FirstOrDefaultAsync(m => m.MenuCategoryId == menuCategoryId);
            if (existing == null)
            {
                return false;
            }

            _context.MenuCategories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MenuCategoryNameExistsAsync(string menuCategoryName, int roleId, int? excludeMenuCategoryId = null)
        {
            return await _context.MenuCategories.AnyAsync(m =>
                m.MenuCategoryName == menuCategoryName && m.RoleId == roleId && m.MenuCategoryId != excludeMenuCategoryId);
        }
    }
}
