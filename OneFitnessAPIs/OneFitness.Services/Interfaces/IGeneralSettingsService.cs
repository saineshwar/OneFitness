using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IGeneralSettingsService
    {
        Task<IReadOnlyList<GeneralSettingsViewModel>> GetAllAsync();
        Task<GeneralSettingsViewModel?> GetByIdAsync(int companyId);
        Task<GeneralSettingsViewModel?> GetActiveAsync();
        Task<GeneralSettingsServiceResult<GeneralSettingsViewModel>> CreateAsync(CreateGeneralSettingsViewModel model);
        Task<GeneralSettingsServiceResult<GeneralSettingsViewModel>> UpdateAsync(int companyId, UpdateGeneralSettingsViewModel model);
        Task<bool> DeleteAsync(int companyId);
    }

    public class GeneralSettingsServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static GeneralSettingsServiceResult<T> Success(T data) => new GeneralSettingsServiceResult<T> { Succeeded = true, Data = data };
        public static GeneralSettingsServiceResult<T> Failure(string error) => new GeneralSettingsServiceResult<T> { Succeeded = false, Error = error };
    }
}
