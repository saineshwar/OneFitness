using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IReceiptService
    {
        Task<IReadOnlyList<ReceiptHistoryViewModel>> GetAllHistoryAsync();
        Task<PagedResultViewModel<ReceiptHistoryViewModel>> GetPagedHistoryAsync(int page, int pageSize, string? search);
        Task<ReceiptServiceResult<ReceiptViewModel>> GenerateAsync(long memberId, int? createdBy);
        Task<ReceiptServiceResult<ReceiptViewModel>> GetHistoryDetailAsync(long receiptHistoryId);
    }

    public class ReceiptServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static ReceiptServiceResult<T> Success(T data) => new ReceiptServiceResult<T> { Succeeded = true, Data = data };
        public static ReceiptServiceResult<T> Failure(string error) => new ReceiptServiceResult<T> { Succeeded = false, Error = error };
    }
}
