using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IPaymentRepository
    {
        Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
        Task<IReadOnlyList<Payment>> GetByMemberIdAsync(long memberId);
        Task<Payment?> GetLatestByMemberIdAsync(long memberId);
        Task<Payment?> GetByIdAsync(long paymentId);
        Task<Payment> AddAsync(Payment payment);
        Task<bool> UpdateAsync(Payment payment);
        Task<long> GetNextInvoiceNoAsync();
    }
}
