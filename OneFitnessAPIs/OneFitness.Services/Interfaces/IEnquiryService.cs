using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IEnquiryService
    {
        Task<IReadOnlyList<EnquiryViewModel>> GetAllAsync();
        Task<EnquiryViewModel?> GetByIdAsync(int enquiryId);
        Task<EnquiryServiceResult<EnquiryViewModel>> CreateAsync(CreateEnquiryViewModel model);
        Task<EnquiryServiceResult<EnquiryViewModel>> UpdateAsync(int enquiryId, UpdateEnquiryViewModel model);
        Task<bool> DeleteAsync(int enquiryId);
    }

    public class EnquiryServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static EnquiryServiceResult<T> Success(T data) => new EnquiryServiceResult<T> { Succeeded = true, Data = data };
        public static EnquiryServiceResult<T> Failure(string error) => new EnquiryServiceResult<T> { Succeeded = false, Error = error };
    }
}
