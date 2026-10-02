using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IRoleMasterRepository
    {
        Task<IReadOnlyList<RoleMaster>> GetAllAsync();
        Task<IReadOnlyList<RoleMaster>> GetAllActiveAsync(int? excludeRoleId = null);
        Task<RoleMaster?> GetByIdAsync(int roleId);
        Task<RoleMaster> AddAsync(RoleMaster roleMaster);
        Task<bool> UpdateAsync(RoleMaster roleMaster);
        Task<bool> DeleteAsync(int roleId);
        Task<bool> RoleNameExistsAsync(string roleName, int? excludeRoleId = null);
    }
}
