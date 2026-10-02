using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class InstallmentRepository : IInstallmentRepository
    {
        private readonly ApplicationDbContext _context;

        public InstallmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Installment>> GetAllAsync()
        {
            return await _context.Installments.AsNoTracking().OrderBy(i => i.InstallmentId).ToListAsync();
        }

        public async Task<Installment?> GetByIdAsync(int installmentId)
        {
            return await _context.Installments.AsNoTracking().FirstOrDefaultAsync(i => i.InstallmentId == installmentId);
        }

        public async Task<Installment> AddAsync(Installment installment)
        {
            _context.Installments.Add(installment);
            await _context.SaveChangesAsync();
            return installment;
        }

        public async Task<bool> UpdateAsync(Installment installment)
        {
            var existing = await _context.Installments.FirstOrDefaultAsync(i => i.InstallmentId == installment.InstallmentId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(installment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int installmentId)
        {
            var existing = await _context.Installments.FirstOrDefaultAsync(i => i.InstallmentId == installmentId);
            if (existing == null)
            {
                return false;
            }

            _context.Installments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InstallmentNameExistsAsync(string installmentName, int? excludeInstallmentId = null)
        {
            return await _context.Installments.AnyAsync(i => i.InstallmentName == installmentName && i.InstallmentId != excludeInstallmentId);
        }
    }
}
