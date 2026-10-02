using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IGeneralSettingsRepository
    {
        Task<IReadOnlyList<GeneralSettings>> GetAllAsync();
        Task<GeneralSettings?> GetByIdAsync(int companyId);
        Task<GeneralSettings?> GetActiveAsync();
        Task<GeneralSettings> AddAsync(GeneralSettings generalSettings);
        Task<bool> UpdateAsync(GeneralSettings generalSettings);
        Task<bool> DeleteAsync(int companyId);
    }
}
