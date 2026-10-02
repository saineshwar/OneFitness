using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IInstallmentService
    {
        Task<IReadOnlyList<InstallmentViewModel>> GetAllAsync();
        Task<InstallmentViewModel?> GetByIdAsync(int installmentId);
        Task<InstallmentServiceResult<InstallmentViewModel>> CreateAsync(CreateInstallmentViewModel model);
        Task<InstallmentServiceResult<InstallmentViewModel>> UpdateAsync(int installmentId, UpdateInstallmentViewModel model);
        Task<bool> DeleteAsync(int installmentId);
    }

    public class InstallmentServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static InstallmentServiceResult<T> Success(T data) => new InstallmentServiceResult<T> { Succeeded = true, Data = data };
        public static InstallmentServiceResult<T> Failure(string error) => new InstallmentServiceResult<T> { Succeeded = false, Error = error };
    }
}
