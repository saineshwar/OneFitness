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
    public class MenuMasterService : IMenuMasterService
    {
        private readonly IMenuMasterRepository _menuMasterRepository;

        public MenuMasterService(IMenuMasterRepository menuMasterRepository)
        {
            _menuMasterRepository = menuMasterRepository;
        }

        public async Task<IReadOnlyList<MenuMasterViewModel>> GetAllAsync()
        {
            var menuMasters = await _menuMasterRepository.GetAllAsync();
            return menuMasters.Select(ToViewModel).ToList();
        }

        public async Task<MenuMasterViewModel?> GetByIdAsync(int menuId)
        {
            var menuMaster = await _menuMasterRepository.GetByIdAsync(menuId);
            return menuMaster == null ? null : ToViewModel(menuMaster);
        }

        public async Task<MenuMasterServiceResult<MenuMasterViewModel>> CreateAsync(CreateMenuMasterViewModel model)
        {
            if (await _menuMasterRepository.MenuNameExistsAsync(model.MenuName, model.RoleId, model.MenuCategoryId))
            {
                return MenuMasterServiceResult<MenuMasterViewModel>.Failure("Menu already exists.");
            }

            var menuMaster = new MenuMaster
            {
                MenuName = model.MenuName,
                Area = model.Area,
                ControllerName = model.ControllerName,
                ActionMethod = model.ActionMethod,
                Status = model.Status,
                MenuCategoryId = model.MenuCategoryId,
                RoleId = model.RoleId,
                SortingOrder = model.SortingOrder,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _menuMasterRepository.AddAsync(menuMaster);
            return MenuMasterServiceResult<MenuMasterViewModel>.Success(ToViewModel(created));
        }

        public async Task<MenuMasterServiceResult<MenuMasterViewModel>> UpdateAsync(int menuId, UpdateMenuMasterViewModel model)
        {
            var existing = await _menuMasterRepository.GetByIdAsync(menuId);
            if (existing == null)
            {
                return MenuMasterServiceResult<MenuMasterViewModel>.Failure("Menu not found.");
            }

            if (await _menuMasterRepository.MenuNameExistsAsync(model.MenuName, model.RoleId, model.MenuCategoryId, menuId))
            {
                return MenuMasterServiceResult<MenuMasterViewModel>.Failure("Menu already exists.");
            }

            existing.MenuName = model.MenuName;
            existing.Area = model.Area;
            existing.ControllerName = model.ControllerName;
            existing.ActionMethod = model.ActionMethod;
            existing.Status = model.Status;
            existing.MenuCategoryId = model.MenuCategoryId;
            existing.RoleId = model.RoleId;
            existing.SortingOrder = model.SortingOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            await _menuMasterRepository.UpdateAsync(existing);
            return MenuMasterServiceResult<MenuMasterViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int menuId)
        {
            return _menuMasterRepository.DeleteAsync(menuId);
        }

        private static MenuMasterViewModel ToViewModel(MenuMaster menuMaster)
        {
            return new MenuMasterViewModel
            {
                MenuId = menuMaster.MenuId,
                MenuName = menuMaster.MenuName,
                Area = menuMaster.Area,
                ControllerName = menuMaster.ControllerName,
                ActionMethod = menuMaster.ActionMethod,
                Status = menuMaster.Status,
                MenuCategoryId = menuMaster.MenuCategoryId,
                RoleId = menuMaster.RoleId,
                SortingOrder = menuMaster.SortingOrder,
                CreatedOn = menuMaster.CreatedOn,
                ModifiedOn = menuMaster.ModifiedOn
            };
        }
    }
}
