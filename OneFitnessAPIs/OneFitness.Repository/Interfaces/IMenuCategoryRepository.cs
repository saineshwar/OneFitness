using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IMenuCategoryRepository
    {
        Task<IReadOnlyList<MenuCategory>> GetAllAsync();
        Task<MenuCategory?> GetByIdAsync(int menuCategoryId);
        Task<MenuCategory> AddAsync(MenuCategory menuCategory);
        Task<bool> UpdateAsync(MenuCategory menuCategory);
        Task<bool> DeleteAsync(int menuCategoryId);
        Task<bool> MenuCategoryNameExistsAsync(string menuCategoryName, int roleId, int? excludeMenuCategoryId = null);
    }
}
