using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IMenuMasterService
    {
        Task<IReadOnlyList<MenuMasterViewModel>> GetAllAsync();
        Task<MenuMasterViewModel?> GetByIdAsync(int menuId);
        Task<MenuMasterServiceResult<MenuMasterViewModel>> CreateAsync(CreateMenuMasterViewModel model);
        Task<MenuMasterServiceResult<MenuMasterViewModel>> UpdateAsync(int menuId, UpdateMenuMasterViewModel model);
        Task<bool> DeleteAsync(int menuId);
    }

    public class MenuMasterServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static MenuMasterServiceResult<T> Success(T data) => new MenuMasterServiceResult<T> { Succeeded = true, Data = data };
        public static MenuMasterServiceResult<T> Failure(string error) => new MenuMasterServiceResult<T> { Succeeded = false, Error = error };
    }
}
