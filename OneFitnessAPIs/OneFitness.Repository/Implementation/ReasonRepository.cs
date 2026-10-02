using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class ReasonRepository : IReasonRepository
    {
        private readonly ApplicationDbContext _context;

        public ReasonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Reason>> GetAllAsync()
        {
            return await _context.Reasons.AsNoTracking().OrderBy(r => r.ReasonId).ToListAsync();
        }

        public async Task<Reason?> GetByIdAsync(int reasonId)
        {
            return await _context.Reasons.AsNoTracking().FirstOrDefaultAsync(r => r.ReasonId == reasonId);
        }

        public async Task<Reason> AddAsync(Reason reason)
        {
            _context.Reasons.Add(reason);
            await _context.SaveChangesAsync();
            return reason;
        }

        public async Task<bool> UpdateAsync(Reason reason)
        {
            var existing = await _context.Reasons.FirstOrDefaultAsync(r => r.ReasonId == reason.ReasonId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(reason);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int reasonId)
        {
            var existing = await _context.Reasons.FirstOrDefaultAsync(r => r.ReasonId == reasonId);
            if (existing == null)
            {
                return false;
            }

            _context.Reasons.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReasonNameExistsAsync(string reasonName, int? excludeReasonId = null)
        {
            return await _context.Reasons.AnyAsync(r => r.ReasonName == reasonName && r.ReasonId != excludeReasonId);
        }
    }
}
