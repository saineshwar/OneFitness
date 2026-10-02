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
    public class TaxMasterService : ITaxMasterService
    {
        private readonly ITaxMasterRepository _taxMasterRepository;

        public TaxMasterService(ITaxMasterRepository taxMasterRepository)
        {
            _taxMasterRepository = taxMasterRepository;
        }

        public async Task<IReadOnlyList<TaxMasterViewModel>> GetAllAsync()
        {
            var taxMasters = await _taxMasterRepository.GetAllAsync();
            return taxMasters.Select(ToViewModel).ToList();
        }

        public async Task<TaxMasterViewModel?> GetByIdAsync(int taxId)
        {
            var taxMaster = await _taxMasterRepository.GetByIdAsync(taxId);
            return taxMaster == null ? null : ToViewModel(taxMaster);
        }

        public async Task<TaxMasterServiceResult<TaxMasterViewModel>> CreateAsync(CreateTaxMasterViewModel model)
        {
            if (await _taxMasterRepository.TaxTypeExistsAsync(model.TaxType))
            {
                return TaxMasterServiceResult<TaxMasterViewModel>.Failure("TaxType already exists.");
            }

            var taxMaster = new TaxMaster
            {
                TaxType = model.TaxType,
                TaxRate = model.TaxRate,
                Status = model.Status,
                IdentificationNo = model.IdentificationNo,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _taxMasterRepository.AddAsync(taxMaster);
            return TaxMasterServiceResult<TaxMasterViewModel>.Success(ToViewModel(created));
        }

        public async Task<TaxMasterServiceResult<TaxMasterViewModel>> UpdateAsync(int taxId, UpdateTaxMasterViewModel model)
        {
            var existing = await _taxMasterRepository.GetByIdAsync(taxId);
            if (existing == null)
            {
                return TaxMasterServiceResult<TaxMasterViewModel>.Failure("TaxMaster not found.");
            }

            if (await _taxMasterRepository.TaxTypeExistsAsync(model.TaxType, taxId))
            {
                return TaxMasterServiceResult<TaxMasterViewModel>.Failure("TaxType already exists.");
            }

            existing.TaxType = model.TaxType;
            existing.TaxRate = model.TaxRate;
            existing.Status = model.Status;
            existing.IdentificationNo = model.IdentificationNo;
            existing.ModifiedOn = DateTime.UtcNow;

            await _taxMasterRepository.UpdateAsync(existing);
            return TaxMasterServiceResult<TaxMasterViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int taxId)
        {
            return _taxMasterRepository.DeleteAsync(taxId);
        }

        private static TaxMasterViewModel ToViewModel(TaxMaster taxMaster)
        {
            return new TaxMasterViewModel
            {
                TaxId = taxMaster.TaxId,
                TaxType = taxMaster.TaxType,
                TaxRate = taxMaster.TaxRate,
                Status = taxMaster.Status,
                IdentificationNo = taxMaster.IdentificationNo,
                CreatedOn = taxMaster.CreatedOn,
                ModifiedOn = taxMaster.ModifiedOn
            };
        }
    }
}
