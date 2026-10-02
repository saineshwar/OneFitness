using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IReceiptHistoryRepository
    {
        Task<IReadOnlyList<ReceiptHistory>> GetAllAsync();
        Task<(IReadOnlyList<ReceiptHistory> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
        Task<ReceiptHistory?> GetByIdAsync(long receiptHistoryId);
        Task<ReceiptHistory> AddAsync(ReceiptHistory receiptHistory);
    }
}
