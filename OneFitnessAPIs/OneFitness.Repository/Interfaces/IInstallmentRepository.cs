using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IInstallmentRepository
    {
        Task<IReadOnlyList<Installment>> GetAllAsync();
        Task<Installment?> GetByIdAsync(int installmentId);
        Task<Installment> AddAsync(Installment installment);
        Task<bool> UpdateAsync(Installment installment);
        Task<bool> DeleteAsync(int installmentId);
        Task<bool> InstallmentNameExistsAsync(string installmentName, int? excludeInstallmentId = null);
    }
}
