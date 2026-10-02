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
    public class SubMenuMasterService : ISubMenuMasterService
    {
        private readonly ISubMenuMasterRepository _subMenuMasterRepository;

        public SubMenuMasterService(ISubMenuMasterRepository subMenuMasterRepository)
        {
            _subMenuMasterRepository = subMenuMasterRepository;
        }

        public async Task<IReadOnlyList<SubMenuMasterViewModel>> GetAllAsync()
        {
            var subMenuMasters = await _subMenuMasterRepository.GetAllAsync();
            return subMenuMasters.Select(ToViewModel).ToList();
        }

        public async Task<SubMenuMasterViewModel?> GetByIdAsync(int subMenuId)
        {
            var subMenuMaster = await _subMenuMasterRepository.GetByIdAsync(subMenuId);
            return subMenuMaster == null ? null : ToViewModel(subMenuMaster);
        }

        public async Task<SubMenuMasterServiceResult<SubMenuMasterViewModel>> CreateAsync(CreateSubMenuMasterViewModel model)
        {
            if (await _subMenuMasterRepository.SubMenuNameExistsAsync(model.SubMenuName, model.MenuId, model.RoleId, model.MenuCategoryId))
            {
                return SubMenuMasterServiceResult<SubMenuMasterViewModel>.Failure("SubMenu already exists.");
            }

            var subMenuMaster = new SubMenuMaster
            {
                SubMenuName = model.SubMenuName,
                Area = model.Area,
                ControllerName = model.ControllerName,
                ActionMethod = model.ActionMethod,
                Status = model.Status,
                MenuId = model.MenuId,
                MenuCategoryId = model.MenuCategoryId,
                RoleId = model.RoleId,
                SortingOrder = model.SortingOrder,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _subMenuMasterRepository.AddAsync(subMenuMaster);
            return SubMenuMasterServiceResult<SubMenuMasterViewModel>.Success(ToViewModel(created));
        }

        public async Task<SubMenuMasterServiceResult<SubMenuMasterViewModel>> UpdateAsync(int subMenuId, UpdateSubMenuMasterViewModel model)
        {
            var existing = await _subMenuMasterRepository.GetByIdAsync(subMenuId);
            if (existing == null)
            {
                return SubMenuMasterServiceResult<SubMenuMasterViewModel>.Failure("SubMenu not found.");
            }

            if (await _subMenuMasterRepository.SubMenuNameExistsAsync(model.SubMenuName, model.MenuId, model.RoleId, model.MenuCategoryId, subMenuId))
            {
                return SubMenuMasterServiceResult<SubMenuMasterViewModel>.Failure("SubMenu already exists.");
            }

            existing.SubMenuName = model.SubMenuName;
            existing.Area = model.Area;
            existing.ControllerName = model.ControllerName;
            existing.ActionMethod = model.ActionMethod;
            existing.Status = model.Status;
            existing.MenuId = model.MenuId;
            existing.MenuCategoryId = model.MenuCategoryId;
            existing.RoleId = model.RoleId;
            existing.SortingOrder = model.SortingOrder;
            existing.ModifiedOn = DateTime.UtcNow;

            await _subMenuMasterRepository.UpdateAsync(existing);
            return SubMenuMasterServiceResult<SubMenuMasterViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int subMenuId)
        {
            return _subMenuMasterRepository.DeleteAsync(subMenuId);
        }

        private static SubMenuMasterViewModel ToViewModel(SubMenuMaster subMenuMaster)
        {
            return new SubMenuMasterViewModel
            {
                SubMenuId = subMenuMaster.SubMenuId,
                SubMenuName = subMenuMaster.SubMenuName,
                Area = subMenuMaster.Area,
                ControllerName = subMenuMaster.ControllerName,
                ActionMethod = subMenuMaster.ActionMethod,
                Status = subMenuMaster.Status,
                MenuId = subMenuMaster.MenuId,
                MenuCategoryId = subMenuMaster.MenuCategoryId,
                RoleId = subMenuMaster.RoleId,
                SortingOrder = subMenuMaster.SortingOrder,
                CreatedOn = subMenuMaster.CreatedOn,
                ModifiedOn = subMenuMaster.ModifiedOn
            };
        }
    }
}
