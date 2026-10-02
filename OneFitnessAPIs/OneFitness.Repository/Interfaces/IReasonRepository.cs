using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IReasonRepository
    {
        Task<IReadOnlyList<Reason>> GetAllAsync();
        Task<Reason?> GetByIdAsync(int reasonId);
        Task<Reason> AddAsync(Reason reason);
        Task<bool> UpdateAsync(Reason reason);
        Task<bool> DeleteAsync(int reasonId);
        Task<bool> ReasonNameExistsAsync(string reasonName, int? excludeReasonId = null);
    }
}
