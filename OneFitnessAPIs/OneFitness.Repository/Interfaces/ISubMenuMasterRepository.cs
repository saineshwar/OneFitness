using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface ISubMenuMasterRepository
    {
        Task<IReadOnlyList<SubMenuMaster>> GetAllAsync();
        Task<SubMenuMaster?> GetByIdAsync(int subMenuId);
        Task<SubMenuMaster> AddAsync(SubMenuMaster subMenuMaster);
        Task<bool> UpdateAsync(SubMenuMaster subMenuMaster);
        Task<bool> DeleteAsync(int subMenuId);
        Task<bool> SubMenuNameExistsAsync(string subMenuName, int menuId, int roleId, int menuCategoryId, int? excludeSubMenuId = null);
    }
}
