using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IMenuCategoryService
    {
        Task<IReadOnlyList<MenuCategoryViewModel>> GetAllAsync();
        Task<MenuCategoryViewModel?> GetByIdAsync(int menuCategoryId);
        Task<MenuCategoryServiceResult<MenuCategoryViewModel>> CreateAsync(CreateMenuCategoryViewModel model);
        Task<MenuCategoryServiceResult<MenuCategoryViewModel>> UpdateAsync(int menuCategoryId, UpdateMenuCategoryViewModel model);
        Task<bool> DeleteAsync(int menuCategoryId);
    }

    public class MenuCategoryServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static MenuCategoryServiceResult<T> Success(T data) => new MenuCategoryServiceResult<T> { Succeeded = true, Data = data };
        public static MenuCategoryServiceResult<T> Failure(string error) => new MenuCategoryServiceResult<T> { Succeeded = false, Error = error };
    }
}
