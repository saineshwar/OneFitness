using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IRoleMasterService
    {
        Task<IReadOnlyList<RoleMasterViewModel>> GetAllAsync();
        Task<IReadOnlyList<RoleMasterViewModel>> GetAllActiveAsync(int? excludeRoleId = null);
        Task<RoleMasterViewModel?> GetByIdAsync(int roleId);
        Task<RoleMasterServiceResult<RoleMasterViewModel>> CreateAsync(CreateRoleMasterViewModel model);
        Task<RoleMasterServiceResult<RoleMasterViewModel>> UpdateAsync(int roleId, UpdateRoleMasterViewModel model);
        Task<bool> DeleteAsync(int roleId);
    }

    public class RoleMasterServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static RoleMasterServiceResult<T> Success(T data) => new RoleMasterServiceResult<T> { Succeeded = true, Data = data };
        public static RoleMasterServiceResult<T> Failure(string error) => new RoleMasterServiceResult<T> { Succeeded = false, Error = error };
    }
}
