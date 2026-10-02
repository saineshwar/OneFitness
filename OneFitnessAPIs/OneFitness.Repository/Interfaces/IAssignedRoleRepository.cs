using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IAssignedRoleRepository
    {
        Task<IReadOnlyList<AssignedRole>> GetAllAsync();
        Task<AssignedRole?> GetByUserIdAsync(int userId);
        Task<AssignedRole> AddAsync(AssignedRole assignedRole);
        Task<bool> UpdateAsync(AssignedRole assignedRole);
    }
}
