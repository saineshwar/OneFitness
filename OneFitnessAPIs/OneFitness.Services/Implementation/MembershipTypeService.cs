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
    public class MembershipTypeService : IMembershipTypeService
    {
        private readonly IMembershipTypeRepository _membershipTypeRepository;

        public MembershipTypeService(IMembershipTypeRepository membershipTypeRepository)
        {
            _membershipTypeRepository = membershipTypeRepository;
        }

        public async Task<IReadOnlyList<MembershipTypeViewModel>> GetAllAsync()
        {
            var membershipTypes = await _membershipTypeRepository.GetAllAsync();
            return membershipTypes.Select(ToViewModel).ToList();
        }

        public async Task<MembershipTypeViewModel?> GetByIdAsync(int membershipTypeId)
        {
            var membershipType = await _membershipTypeRepository.GetByIdAsync(membershipTypeId);
            return membershipType == null ? null : ToViewModel(membershipType);
        }

        public async Task<MembershipTypeServiceResult<MembershipTypeViewModel>> CreateAsync(CreateMembershipTypeViewModel model)
        {
            if (await _membershipTypeRepository.MembershipTypeNameExistsAsync(model.MembershipTypeName))
            {
                return MembershipTypeServiceResult<MembershipTypeViewModel>.Failure("MembershipType already exists.");
            }

            var membershipType = new MembershipType
            {
                MembershipTypeName = model.MembershipTypeName,
                Amount = model.Amount,
                InstallmentId = model.InstallmentId,
                WorkOutId = model.WorkOutId,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _membershipTypeRepository.AddAsync(membershipType);
            return MembershipTypeServiceResult<MembershipTypeViewModel>.Success(ToViewModel(created));
        }

        public async Task<MembershipTypeServiceResult<MembershipTypeViewModel>> UpdateAsync(int membershipTypeId, UpdateMembershipTypeViewModel model)
        {
            var existing = await _membershipTypeRepository.GetByIdAsync(membershipTypeId);
            if (existing == null)
            {
                return MembershipTypeServiceResult<MembershipTypeViewModel>.Failure("MembershipType not found.");
            }

            if (await _membershipTypeRepository.MembershipTypeNameExistsAsync(model.MembershipTypeName, membershipTypeId))
            {
                return MembershipTypeServiceResult<MembershipTypeViewModel>.Failure("MembershipType already exists.");
            }

            existing.MembershipTypeName = model.MembershipTypeName;
            existing.Amount = model.Amount;
            existing.InstallmentId = model.InstallmentId;
            existing.WorkOutId = model.WorkOutId;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _membershipTypeRepository.UpdateAsync(existing);
            return MembershipTypeServiceResult<MembershipTypeViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int membershipTypeId)
        {
            return _membershipTypeRepository.DeleteAsync(membershipTypeId);
        }

        private static MembershipTypeViewModel ToViewModel(MembershipType membershipType)
        {
            return new MembershipTypeViewModel
            {
                MembershipTypeId = membershipType.MembershipTypeId,
                MembershipTypeName = membershipType.MembershipTypeName,
                Amount = membershipType.Amount,
                InstallmentId = membershipType.InstallmentId,
                WorkOutId = membershipType.WorkOutId,
                Status = membershipType.Status,
                CreatedOn = membershipType.CreatedOn,
                ModifiedOn = membershipType.ModifiedOn
            };
        }
    }
}
