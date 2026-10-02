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
    public class EnquiryService : IEnquiryService
    {
        private readonly IEnquiryRepository _enquiryRepository;

        public EnquiryService(IEnquiryRepository enquiryRepository)
        {
            _enquiryRepository = enquiryRepository;
        }

        public async Task<IReadOnlyList<EnquiryViewModel>> GetAllAsync()
        {
            var enquiries = await _enquiryRepository.GetAllAsync();
            return enquiries.Select(ToViewModel).ToList();
        }

        public async Task<EnquiryViewModel?> GetByIdAsync(int enquiryId)
        {
            var enquiry = await _enquiryRepository.GetByIdAsync(enquiryId);
            return enquiry == null ? null : ToViewModel(enquiry);
        }

        public async Task<EnquiryServiceResult<EnquiryViewModel>> CreateAsync(CreateEnquiryViewModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.MobileNo) && await _enquiryRepository.MobileNoExistsAsync(model.MobileNo))
            {
                return EnquiryServiceResult<EnquiryViewModel>.Failure("MobileNo already exists.");
            }

            if (!string.IsNullOrWhiteSpace(model.EmailId) && await _enquiryRepository.EmailExistsAsync(model.EmailId))
            {
                return EnquiryServiceResult<EnquiryViewModel>.Failure("EmailId already exists.");
            }

            var enquiry = new Enquiry
            {
                WorkOutId = model.WorkOutId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                MobileNo = model.MobileNo,
                EmailId = model.EmailId,
                GenderId = model.GenderId,
                ReasonId = model.ReasonId,
                EnquiryDetails = model.EnquiryDetails,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _enquiryRepository.AddAsync(enquiry);
            return EnquiryServiceResult<EnquiryViewModel>.Success(ToViewModel(created));
        }

        public async Task<EnquiryServiceResult<EnquiryViewModel>> UpdateAsync(int enquiryId, UpdateEnquiryViewModel model)
        {
            var existing = await _enquiryRepository.GetByIdAsync(enquiryId);
            if (existing == null)
            {
                return EnquiryServiceResult<EnquiryViewModel>.Failure("Enquiry not found.");
            }

            if (!string.IsNullOrWhiteSpace(model.MobileNo) && await _enquiryRepository.MobileNoExistsAsync(model.MobileNo, enquiryId))
            {
                return EnquiryServiceResult<EnquiryViewModel>.Failure("MobileNo already exists.");
            }

            if (!string.IsNullOrWhiteSpace(model.EmailId) && await _enquiryRepository.EmailExistsAsync(model.EmailId, enquiryId))
            {
                return EnquiryServiceResult<EnquiryViewModel>.Failure("EmailId already exists.");
            }

            existing.WorkOutId = model.WorkOutId;
            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.MiddleName = model.MiddleName;
            existing.MobileNo = model.MobileNo;
            existing.EmailId = model.EmailId;
            existing.GenderId = model.GenderId;
            existing.ReasonId = model.ReasonId;
            existing.EnquiryDetails = model.EnquiryDetails;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _enquiryRepository.UpdateAsync(existing);
            return EnquiryServiceResult<EnquiryViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int enquiryId)
        {
            return _enquiryRepository.DeleteAsync(enquiryId);
        }

        private static EnquiryViewModel ToViewModel(Enquiry enquiry)
        {
            return new EnquiryViewModel
            {
                EnquiryId = enquiry.EnquiryId,
                WorkOutId = enquiry.WorkOutId,
                FirstName = enquiry.FirstName,
                LastName = enquiry.LastName,
                MiddleName = enquiry.MiddleName,
                MobileNo = enquiry.MobileNo,
                EmailId = enquiry.EmailId,
                GenderId = enquiry.GenderId,
                ReasonId = enquiry.ReasonId,
                EnquiryDetails = enquiry.EnquiryDetails,
                Status = enquiry.Status,
                CreatedOn = enquiry.CreatedOn,
                ModifiedOn = enquiry.ModifiedOn
            };
        }
    }
}
