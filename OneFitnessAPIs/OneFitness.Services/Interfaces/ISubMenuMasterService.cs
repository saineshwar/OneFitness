using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface ISubMenuMasterService
    {
        Task<IReadOnlyList<SubMenuMasterViewModel>> GetAllAsync();
        Task<SubMenuMasterViewModel?> GetByIdAsync(int subMenuId);
        Task<SubMenuMasterServiceResult<SubMenuMasterViewModel>> CreateAsync(CreateSubMenuMasterViewModel model);
        Task<SubMenuMasterServiceResult<SubMenuMasterViewModel>> UpdateAsync(int subMenuId, UpdateSubMenuMasterViewModel model);
        Task<bool> DeleteAsync(int subMenuId);
    }

    public class SubMenuMasterServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static SubMenuMasterServiceResult<T> Success(T data) => new SubMenuMasterServiceResult<T> { Succeeded = true, Data = data };
        public static SubMenuMasterServiceResult<T> Failure(string error) => new SubMenuMasterServiceResult<T> { Succeeded = false, Error = error };
    }
}
