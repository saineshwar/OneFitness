using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class ReceiptHistoryRepository : IReceiptHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ReceiptHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ReceiptHistory>> GetAllAsync()
        {
            return await _context.ReceiptHistories.AsNoTracking().OrderByDescending(r => r.ReceiptHistoryId).ToListAsync();
        }

        public async Task<(IReadOnlyList<ReceiptHistory> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var query = _context.ReceiptHistories.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(r => EF.Functions.Like(r.MemberNo, $"%{term}%"));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.ReceiptHistoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ReceiptHistory?> GetByIdAsync(long receiptHistoryId)
        {
            return await _context.ReceiptHistories.AsNoTracking().FirstOrDefaultAsync(r => r.ReceiptHistoryId == receiptHistoryId);
        }

        public async Task<ReceiptHistory> AddAsync(ReceiptHistory receiptHistory)
        {
            _context.ReceiptHistories.Add(receiptHistory);
            await _context.SaveChangesAsync();
            return receiptHistory;
        }
    }
}
