using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface ITaxMasterRepository
    {
        Task<IReadOnlyList<TaxMaster>> GetAllAsync();
        Task<TaxMaster?> GetByIdAsync(int taxId);
        Task<TaxMaster> AddAsync(TaxMaster taxMaster);
        Task<bool> UpdateAsync(TaxMaster taxMaster);
        Task<bool> DeleteAsync(int taxId);
        Task<bool> TaxTypeExistsAsync(string taxType, int? excludeTaxId = null);
    }
}
