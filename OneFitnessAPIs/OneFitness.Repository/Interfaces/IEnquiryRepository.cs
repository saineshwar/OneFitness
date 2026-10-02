using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IEnquiryRepository
    {
        Task<IReadOnlyList<Enquiry>> GetAllAsync();
        Task<Enquiry?> GetByIdAsync(int enquiryId);
        Task<Enquiry> AddAsync(Enquiry enquiry);
        Task<bool> UpdateAsync(Enquiry enquiry);
        Task<bool> DeleteAsync(int enquiryId);
        Task<bool> MobileNoExistsAsync(string mobileNo, int? excludeEnquiryId = null);
        Task<bool> EmailExistsAsync(string emailId, int? excludeEnquiryId = null);

    }
}
