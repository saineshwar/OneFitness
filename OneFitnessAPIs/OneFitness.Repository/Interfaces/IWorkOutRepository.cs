using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IWorkOutRepository
    {
        Task<IReadOnlyList<WorkOut>> GetAllAsync();
        Task<WorkOut?> GetByIdAsync(int workOutId);
        Task<WorkOut> AddAsync(WorkOut workOut);
        Task<bool> UpdateAsync(WorkOut workOut);
        Task<bool> DeleteAsync(int workOutId);
        Task<bool> WorkOutNameExistsAsync(string workOutName, int? excludeWorkOutId = null);
    }
}
