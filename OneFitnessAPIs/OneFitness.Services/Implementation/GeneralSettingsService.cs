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
    public class GeneralSettingsService : IGeneralSettingsService
    {
        private readonly IGeneralSettingsRepository _generalSettingsRepository;

        public GeneralSettingsService(IGeneralSettingsRepository generalSettingsRepository)
        {
            _generalSettingsRepository = generalSettingsRepository;
        }

        public async Task<IReadOnlyList<GeneralSettingsViewModel>> GetAllAsync()
        {
            var settings = await _generalSettingsRepository.GetAllAsync();
            return settings.Select(ToViewModel).ToList();
        }

        public async Task<GeneralSettingsViewModel?> GetByIdAsync(int companyId)
        {
            var settings = await _generalSettingsRepository.GetByIdAsync(companyId);
            return settings == null ? null : ToViewModel(settings);
        }

        public async Task<GeneralSettingsViewModel?> GetActiveAsync()
        {
            var settings = await _generalSettingsRepository.GetActiveAsync();
            return settings == null ? null : ToViewModel(settings);
        }

        public async Task<GeneralSettingsServiceResult<GeneralSettingsViewModel>> CreateAsync(CreateGeneralSettingsViewModel model)
        {
            var generalSettings = new GeneralSettings
            {
                Name = model.Name,
                SupportEmailId = model.SupportEmailId,
                WebsiteTitle = model.WebsiteTitle,
                WebsiteUrl = model.WebsiteUrl,
                TelephoneNo = model.TelephoneNo,
                MobileNo = model.MobileNo,
                Status = model.Status,
                Logopath = model.Logopath,
                LogoFileName = model.LogoFileName,
                Address = model.Address,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _generalSettingsRepository.AddAsync(generalSettings);
            return GeneralSettingsServiceResult<GeneralSettingsViewModel>.Success(ToViewModel(created));
        }

        public async Task<GeneralSettingsServiceResult<GeneralSettingsViewModel>> UpdateAsync(int companyId, UpdateGeneralSettingsViewModel model)
        {
            var existing = await _generalSettingsRepository.GetByIdAsync(companyId);
            if (existing == null)
            {
                return GeneralSettingsServiceResult<GeneralSettingsViewModel>.Failure("GeneralSettings not found.");
            }

            existing.Name = model.Name;
            existing.SupportEmailId = model.SupportEmailId;
            existing.WebsiteTitle = model.WebsiteTitle;
            existing.WebsiteUrl = model.WebsiteUrl;
            existing.TelephoneNo = model.TelephoneNo;
            existing.MobileNo = model.MobileNo;
            existing.Status = model.Status;
            existing.Logopath = model.Logopath;
            existing.LogoFileName = model.LogoFileName;
            existing.Address = model.Address;
            existing.ModifiedOn = DateTime.UtcNow;

            await _generalSettingsRepository.UpdateAsync(existing);
            return GeneralSettingsServiceResult<GeneralSettingsViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int companyId)
        {
            return _generalSettingsRepository.DeleteAsync(companyId);
        }

        private static GeneralSettingsViewModel ToViewModel(GeneralSettings generalSettings)
        {
            return new GeneralSettingsViewModel
            {
                CompanyId = generalSettings.CompanyId,
                Name = generalSettings.Name,
                SupportEmailId = generalSettings.SupportEmailId,
                WebsiteTitle = generalSettings.WebsiteTitle,
                WebsiteUrl = generalSettings.WebsiteUrl,
                TelephoneNo = generalSettings.TelephoneNo,
                MobileNo = generalSettings.MobileNo,
                Status = generalSettings.Status,
                Logopath = generalSettings.Logopath,
                LogoFileName = generalSettings.LogoFileName,
                Address = generalSettings.Address,
                CreatedOn = generalSettings.CreatedOn,
                ModifiedOn = generalSettings.ModifiedOn
            };
        }
    }
}
