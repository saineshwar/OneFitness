using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class TaxMasterRepository : ITaxMasterRepository
    {
        private readonly ApplicationDbContext _context;

        public TaxMasterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TaxMaster>> GetAllAsync()
        {
            return await _context.TaxMasters.AsNoTracking().OrderBy(t => t.TaxId).ToListAsync();
        }

        public async Task<TaxMaster?> GetByIdAsync(int taxId)
        {
            return await _context.TaxMasters.AsNoTracking().FirstOrDefaultAsync(t => t.TaxId == taxId);
        }

        public async Task<TaxMaster> AddAsync(TaxMaster taxMaster)
        {
            _context.TaxMasters.Add(taxMaster);
            await _context.SaveChangesAsync();
            return taxMaster;
        }

        public async Task<bool> UpdateAsync(TaxMaster taxMaster)
        {
            var existing = await _context.TaxMasters.FirstOrDefaultAsync(t => t.TaxId == taxMaster.TaxId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(taxMaster);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int taxId)
        {
            var existing = await _context.TaxMasters.FirstOrDefaultAsync(t => t.TaxId == taxId);
            if (existing == null)
            {
                return false;
            }

            _context.TaxMasters.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TaxTypeExistsAsync(string taxType, int? excludeTaxId = null)
        {
            return await _context.TaxMasters.AnyAsync(t => t.TaxType == taxType && t.TaxId != excludeTaxId);
        }
    }
}
