using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class PaymentTypeRepository : IPaymentTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PaymentType>> GetAllAsync()
        {
            return await _context.PaymentTypes.AsNoTracking().OrderBy(p => p.PaymentTypeId).ToListAsync();
        }

        public async Task<PaymentType?> GetByIdAsync(int paymentTypeId)
        {
            return await _context.PaymentTypes.AsNoTracking().FirstOrDefaultAsync(p => p.PaymentTypeId == paymentTypeId);
        }

        public async Task<PaymentType> AddAsync(PaymentType paymentType)
        {
            _context.PaymentTypes.Add(paymentType);
            await _context.SaveChangesAsync();
            return paymentType;
        }

        public async Task<bool> UpdateAsync(PaymentType paymentType)
        {
            var existing = await _context.PaymentTypes.FirstOrDefaultAsync(p => p.PaymentTypeId == paymentType.PaymentTypeId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(paymentType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int paymentTypeId)
        {
            var existing = await _context.PaymentTypes.FirstOrDefaultAsync(p => p.PaymentTypeId == paymentTypeId);
            if (existing == null)
            {
                return false;
            }

            _context.PaymentTypes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PaymentTypeNameExistsAsync(string paymentTypeName, int? excludePaymentTypeId = null)
        {
            return await _context.PaymentTypes.AnyAsync(p => p.PaymentTypeName == paymentTypeName && p.PaymentTypeId != excludePaymentTypeId);
        }
    }
}
