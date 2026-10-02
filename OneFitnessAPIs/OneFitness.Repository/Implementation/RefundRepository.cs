using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class RefundRepository : IRefundRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Refund>> GetAllAsync()
        {
            return await _context.Refunds.AsNoTracking().OrderByDescending(r => r.RefundId).ToListAsync();
        }

        public async Task<(IReadOnlyList<Refund> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var query = _context.Refunds.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                var matchingMemberIds = _context.Members.AsNoTracking()
                    .Where(m =>
                        EF.Functions.Like(m.FirstName, $"%{term}%") ||
                        (m.LastName != null && EF.Functions.Like(m.LastName, $"%{term}%")) ||
                        EF.Functions.Like(m.MemberNo, $"%{term}%"))
                    .Select(m => m.MemberId);

                query = query.Where(r => matchingMemberIds.Contains(r.MemberId));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.RefundId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Refund?> GetByIdAsync(int refundId)
        {
            return await _context.Refunds.AsNoTracking().FirstOrDefaultAsync(r => r.RefundId == refundId);
        }

        public async Task<Refund> AddAsync(Refund refund)
        {
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();
            return refund;
        }

        public async Task<bool> DeleteAsync(int refundId)
        {
            var existing = await _context.Refunds.FirstOrDefaultAsync(r => r.RefundId == refundId);
            if (existing == null)
            {
                return false;
            }

            _context.Refunds.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefundExistsForMemberAsync(long memberId)
        {
            return await _context.Refunds.AnyAsync(r => r.MemberId == memberId);
        }
    }
}
