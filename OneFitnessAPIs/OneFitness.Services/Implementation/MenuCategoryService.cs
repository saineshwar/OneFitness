using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Services.Implementation
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IMenuCategoryRepository _menuCategoryRepository;

        public MenuCategoryService(IMenuCategoryRepository menuCategoryRepository)
        {
            _menuCategoryRepository = menuCategoryRepository;
        }

        public async Task<IReadOnlyList<MenuCategoryViewModel>> GetAllAsync()
        {
            var menuCategories = await _menuCategoryRepository.GetAllAsync();
            return menuCategories.Select(ToViewModel).ToList();
        }

        public async Task<MenuCategoryViewModel?> GetByIdAsync(int menuCategoryId)
        {
            var menuCategory = await _menuCategoryRepository.GetByIdAsync(menuCategoryId);
            return menuCategory == null ? null : ToViewModel(menuCategory);
        }

        public async Task<MenuCategoryServiceResult<MenuCategoryViewModel>> CreateAsync(CreateMenuCategoryViewModel model)
        {
            if (await _menuCategoryRepository.MenuCategoryNameExistsAsync(model.MenuCategoryName, model.RoleId))
            {
                return MenuCategoryServiceResult<MenuCategoryViewModel>.Failure("MenuCategory already exists.");
            }

            var menuCategory = new MenuCategory
            {
                MenuCategoryName = model.MenuCategoryName,
                RoleId = model.RoleId,
                Status = model.Status,
                SortingOrder = model.SortingOrder,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _menuCategoryRepository.AddAsync(menuCategory);
            return MenuCategoryServiceResult<MenuCategoryViewModel>.Success(ToViewModel(created));
        }

        public async Task<MenuCategoryServiceResult<MenuCategoryViewModel>> UpdateAsync(int menuCategoryId, UpdateMenuCategoryViewModel model)
        {
            var existing = await _menuCategoryRepository.GetByIdAsync(menuCategoryId);
            if (existing == null)
            {
                return MenuCategoryServiceResult<MenuCategoryViewModel>.Failure("MenuCategory not found.");
            }

            if (await _menuCategoryRepository.MenuCategoryNameExistsAsync(model.MenuCategoryName, model.RoleId, menuCategoryId))
            {
                return MenuCategoryServiceResult<MenuCategoryViewModel>.Failure("MenuCategory already exists.");
            }

            existing.MenuCategoryName = model.MenuCategoryName;
            existing.RoleId = model.RoleId;
            existing.Status = model.Status;
            existing.SortingOrder = model.SortingOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            await _menuCategoryRepository.UpdateAsync(existing);
            return MenuCategoryServiceResult<MenuCategoryViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int menuCategoryId)
        {
            return _menuCategoryRepository.DeleteAsync(menuCategoryId);
        }

        private static MenuCategoryViewModel ToViewModel(MenuCategory menuCategory)
        {
            return new MenuCategoryViewModel
            {
                MenuCategoryId = menuCategory.MenuCategoryId,
                MenuCategoryName = menuCategory.MenuCategoryName,
                RoleId = menuCategory.RoleId,
                Status = menuCategory.Status,
                SortingOrder = menuCategory.SortingOrder,
                CreatedOn = menuCategory.CreatedOn,
                ModifiedOn = menuCategory.ModifiedOn
            };
        }
    }
}
