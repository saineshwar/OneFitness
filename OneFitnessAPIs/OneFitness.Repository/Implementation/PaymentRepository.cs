using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var query = _context.Payments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                var matchingMemberIds = _context.Members.AsNoTracking()
                    .Where(m =>
                        EF.Functions.Like(m.FirstName, $"%{term}%") ||
                        (m.LastName != null && EF.Functions.Like(m.LastName, $"%{term}%")) ||
                        EF.Functions.Like(m.MemberNo, $"%{term}%"))
                    .Select(m => m.MemberId);

                query = query.Where(p => matchingMemberIds.Contains(p.MemberId));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.PaymentId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Payment>> GetByMemberIdAsync(long memberId)
        {
            return await _context.Payments.AsNoTracking()
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.PaymentId)
                .ToListAsync();
        }

        public async Task<Payment?> GetLatestByMemberIdAsync(long memberId)
        {
            return await _context.Payments.AsNoTracking()
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.PaymentId)
                .FirstOrDefaultAsync();
        }

        public async Task<Payment?> GetByIdAsync(long paymentId)
        {
            return await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<Payment> AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> UpdateAsync(Payment payment)
        {
            var existing = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == payment.PaymentId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<long> GetNextInvoiceNoAsync()
        {
            var maxInvoiceNo = await _context.Payments.MaxAsync(p => (long?)p.InvoiceNo);
            return (maxInvoiceNo ?? 0) + 1;
        }
    }
}
