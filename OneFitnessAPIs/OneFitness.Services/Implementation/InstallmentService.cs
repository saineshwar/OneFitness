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
    public class InstallmentService : IInstallmentService
    {
        private readonly IInstallmentRepository _installmentRepository;

        public InstallmentService(IInstallmentRepository installmentRepository)
        {
            _installmentRepository = installmentRepository;
        }

        public async Task<IReadOnlyList<InstallmentViewModel>> GetAllAsync()
        {
            var installments = await _installmentRepository.GetAllAsync();
            return installments.Select(ToViewModel).ToList();
        }

        public async Task<InstallmentViewModel?> GetByIdAsync(int installmentId)
        {
            var installment = await _installmentRepository.GetByIdAsync(installmentId);
            return installment == null ? null : ToViewModel(installment);
        }

        public async Task<InstallmentServiceResult<InstallmentViewModel>> CreateAsync(CreateInstallmentViewModel model)
        {
            if (await _installmentRepository.InstallmentNameExistsAsync(model.InstallmentName))
            {
                return InstallmentServiceResult<InstallmentViewModel>.Failure("Installment already exists.");
            }

            var installment = new Installment
            {
                InstallmentName = model.InstallmentName,
                Status = model.Status,
                InstallmentMonths = model.InstallmentMonths,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _installmentRepository.AddAsync(installment);
            return InstallmentServiceResult<InstallmentViewModel>.Success(ToViewModel(created));
        }

        public async Task<InstallmentServiceResult<InstallmentViewModel>> UpdateAsync(int installmentId, UpdateInstallmentViewModel model)
        {
            var existing = await _installmentRepository.GetByIdAsync(installmentId);
            if (existing == null)
            {
                return InstallmentServiceResult<InstallmentViewModel>.Failure("Installment not found.");
            }

            if (await _installmentRepository.InstallmentNameExistsAsync(model.InstallmentName, installmentId))
            {
                return InstallmentServiceResult<InstallmentViewModel>.Failure("Installment already exists.");
            }

            existing.InstallmentName = model.InstallmentName;
            existing.Status = model.Status;
            existing.InstallmentMonths = model.InstallmentMonths;
            existing.ModifiedOn = DateTime.UtcNow;

            await _installmentRepository.UpdateAsync(existing);
            return InstallmentServiceResult<InstallmentViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int installmentId)
        {
            return _installmentRepository.DeleteAsync(installmentId);
        }

        private static InstallmentViewModel ToViewModel(Installment installment)
        {
            return new InstallmentViewModel
            {
                InstallmentId = installment.InstallmentId,
                InstallmentName = installment.InstallmentName,
                Status = installment.Status,
                InstallmentMonths = installment.InstallmentMonths,
                CreatedOn = installment.CreatedOn,
                ModifiedOn = installment.ModifiedOn
            };
        }
    }
}
