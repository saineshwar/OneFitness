using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class GeneralSettingsRepository : IGeneralSettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public GeneralSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<GeneralSettings>> GetAllAsync()
        {
            return await _context.GeneralSettings.AsNoTracking().OrderByDescending(g => g.CompanyId).ToListAsync();
        }

        public async Task<GeneralSettings?> GetByIdAsync(int companyId)
        {
            return await _context.GeneralSettings.AsNoTracking().FirstOrDefaultAsync(g => g.CompanyId == companyId);
        }

        public async Task<GeneralSettings?> GetActiveAsync()
        {
            return await _context.GeneralSettings.AsNoTracking().FirstOrDefaultAsync(g => g.Status);
        }

        public async Task<GeneralSettings> AddAsync(GeneralSettings generalSettings)
        {
            _context.GeneralSettings.Add(generalSettings);
            await _context.SaveChangesAsync();
            return generalSettings;
        }

        public async Task<bool> UpdateAsync(GeneralSettings generalSettings)
        {
            var existing = await _context.GeneralSettings.FirstOrDefaultAsync(g => g.CompanyId == generalSettings.CompanyId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(generalSettings);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int companyId)
        {
            var existing = await _context.GeneralSettings.FirstOrDefaultAsync(g => g.CompanyId == companyId);
            if (existing == null)
            {
                return false;
            }

            _context.GeneralSettings.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
