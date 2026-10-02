using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class WorkOutRepository : IWorkOutRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkOutRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<WorkOut>> GetAllAsync()
        {
            return await _context.WorkOuts.AsNoTracking().OrderBy(w => w.WorkOutId).ToListAsync();
        }

        public async Task<WorkOut?> GetByIdAsync(int workOutId)
        {
            return await _context.WorkOuts.AsNoTracking().FirstOrDefaultAsync(w => w.WorkOutId == workOutId);
        }

        public async Task<WorkOut> AddAsync(WorkOut workOut)
        {
            _context.WorkOuts.Add(workOut);
            await _context.SaveChangesAsync();
            return workOut;
        }

        public async Task<bool> UpdateAsync(WorkOut workOut)
        {
            var existing = await _context.WorkOuts.FirstOrDefaultAsync(w => w.WorkOutId == workOut.WorkOutId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(workOut);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int workOutId)
        {
            var existing = await _context.WorkOuts.FirstOrDefaultAsync(w => w.WorkOutId == workOutId);
            if (existing == null)
            {
                return false;
            }

            _context.WorkOuts.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> WorkOutNameExistsAsync(string workOutName, int? excludeWorkOutId = null)
        {
            return await _context.WorkOuts.AnyAsync(w => w.WorkOutName == workOutName && w.WorkOutId != excludeWorkOutId);
        }
    }
}
