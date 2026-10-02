using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IRefundService
    {
        Task<IReadOnlyList<RefundViewModel>> GetAllAsync();
        Task<PagedResultViewModel<RefundViewModel>> GetPagedAsync(int page, int pageSize, string? search);
        Task<RefundViewModel?> GetByIdAsync(int refundId);
        Task<RefundServiceResult<RefundViewModel>> CreateAsync(CreateRefundViewModel model, int? createdBy);
        Task<bool> DeleteAsync(int refundId);
    }

    public class RefundServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static RefundServiceResult<T> Success(T data) => new RefundServiceResult<T> { Succeeded = true, Data = data };
        public static RefundServiceResult<T> Failure(string error) => new RefundServiceResult<T> { Succeeded = false, Error = error };
    }
}
