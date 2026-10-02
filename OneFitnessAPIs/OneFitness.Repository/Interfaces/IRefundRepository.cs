using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IRefundRepository
    {
        Task<IReadOnlyList<Refund>> GetAllAsync();
        Task<(IReadOnlyList<Refund> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
        Task<Refund?> GetByIdAsync(int refundId);
        Task<Refund> AddAsync(Refund refund);
        Task<bool> DeleteAsync(int refundId);
        Task<bool> RefundExistsForMemberAsync(long memberId);
    }
}
