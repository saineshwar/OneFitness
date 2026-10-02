using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IMenuMasterRepository
    {
        Task<IReadOnlyList<MenuMaster>> GetAllAsync();
        Task<MenuMaster?> GetByIdAsync(int menuId);
        Task<MenuMaster> AddAsync(MenuMaster menuMaster);
        Task<bool> UpdateAsync(MenuMaster menuMaster);
        Task<bool> DeleteAsync(int menuId);
        Task<bool> MenuNameExistsAsync(string menuName, int roleId, int menuCategoryId, int? excludeMenuId = null);
    }
}
