using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class EnquiryRepository : IEnquiryRepository
    {
        private readonly ApplicationDbContext _context;

        public EnquiryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Enquiry>> GetAllAsync()
        {
            return await _context.Enquiries.AsNoTracking().OrderByDescending(e => e.EnquiryId).ToListAsync();
        }

        public async Task<Enquiry?> GetByIdAsync(int enquiryId)
        {
            return await _context.Enquiries.AsNoTracking().FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);
        }

        public async Task<Enquiry> AddAsync(Enquiry enquiry)
        {
            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync();
            return enquiry;
        }

        public async Task<bool> UpdateAsync(Enquiry enquiry)
        {
            var existing = await _context.Enquiries.FirstOrDefaultAsync(e => e.EnquiryId == enquiry.EnquiryId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(enquiry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int enquiryId)
        {
            var existing = await _context.Enquiries.FirstOrDefaultAsync(e => e.EnquiryId == enquiryId);
            if (existing == null)
            {
                return false;
            }

            _context.Enquiries.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MobileNoExistsAsync(string mobileNo, int? excludeEnquiryId = null)
        {
            return await _context.Enquiries.AnyAsync(e => e.MobileNo == mobileNo && e.EnquiryId != excludeEnquiryId);
        }

        public async Task<bool> EmailExistsAsync(string emailId, int? excludeEnquiryId = null)
        {
            return await _context.Enquiries.AnyAsync(e => e.EmailId == emailId && e.EnquiryId != excludeEnquiryId);
        }
    }
}
