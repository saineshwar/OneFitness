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
    public class RoleMasterService : IRoleMasterService
    {
        private readonly IRoleMasterRepository _roleMasterRepository;

        public RoleMasterService(IRoleMasterRepository roleMasterRepository)
        {
            _roleMasterRepository = roleMasterRepository;
        }

        public async Task<IReadOnlyList<RoleMasterViewModel>> GetAllAsync()
        {
            var roles = await _roleMasterRepository.GetAllAsync();
            return roles.Select(ToViewModel).ToList();
        }

        public async Task<IReadOnlyList<RoleMasterViewModel>> GetAllActiveAsync(int? excludeRoleId = null)
        {
            var roles = await _roleMasterRepository.GetAllActiveAsync(excludeRoleId);
            return roles.Select(ToViewModel).ToList();
        }

        public async Task<RoleMasterViewModel?> GetByIdAsync(int roleId)
        {
            var role = await _roleMasterRepository.GetByIdAsync(roleId);
            return role == null ? null : ToViewModel(role);
        }

        public async Task<RoleMasterServiceResult<RoleMasterViewModel>> CreateAsync(CreateRoleMasterViewModel model)
        {
            if (await _roleMasterRepository.RoleNameExistsAsync(model.RoleName))
            {
                return RoleMasterServiceResult<RoleMasterViewModel>.Failure("Role already exists.");
            }

            var role = new RoleMaster
            {
                RoleName = model.RoleName,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _roleMasterRepository.AddAsync(role);
            return RoleMasterServiceResult<RoleMasterViewModel>.Success(ToViewModel(created));
        }

        public async Task<RoleMasterServiceResult<RoleMasterViewModel>> UpdateAsync(int roleId, UpdateRoleMasterViewModel model)
        {
            var existing = await _roleMasterRepository.GetByIdAsync(roleId);
            if (existing == null)
            {
                return RoleMasterServiceResult<RoleMasterViewModel>.Failure("Role not found.");
            }

            if (await _roleMasterRepository.RoleNameExistsAsync(model.RoleName, roleId))
            {
                return RoleMasterServiceResult<RoleMasterViewModel>.Failure("Role already exists.");
            }

            existing.RoleName = model.RoleName;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _roleMasterRepository.UpdateAsync(existing);
            return RoleMasterServiceResult<RoleMasterViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int roleId)
        {
            return _roleMasterRepository.DeleteAsync(roleId);
        }

        private static RoleMasterViewModel ToViewModel(RoleMaster role)
        {
            return new RoleMasterViewModel
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Status = role.Status,
                CreatedOn = role.CreatedOn,
                ModifiedOn = role.ModifiedOn
            };
        }
    }
}
