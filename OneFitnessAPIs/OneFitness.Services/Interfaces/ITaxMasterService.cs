using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface ITaxMasterService
    {
        Task<IReadOnlyList<TaxMasterViewModel>> GetAllAsync();
        Task<TaxMasterViewModel?> GetByIdAsync(int taxId);
        Task<TaxMasterServiceResult<TaxMasterViewModel>> CreateAsync(CreateTaxMasterViewModel model);
        Task<TaxMasterServiceResult<TaxMasterViewModel>> UpdateAsync(int taxId, UpdateTaxMasterViewModel model);
        Task<bool> DeleteAsync(int taxId);
    }

    public class TaxMasterServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static TaxMasterServiceResult<T> Success(T data) => new TaxMasterServiceResult<T> { Succeeded = true, Data = data };
        public static TaxMasterServiceResult<T> Failure(string error) => new TaxMasterServiceResult<T> { Succeeded = false, Error = error };
    }
}
