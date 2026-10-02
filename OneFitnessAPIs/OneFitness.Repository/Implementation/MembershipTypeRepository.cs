using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class MembershipTypeRepository : IMembershipTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public MembershipTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<MembershipType>> GetAllAsync()
        {
            return await _context.MembershipTypes.AsNoTracking().OrderBy(m => m.MembershipTypeId).ToListAsync();
        }

        public async Task<MembershipType?> GetByIdAsync(int membershipTypeId)
        {
            return await _context.MembershipTypes.AsNoTracking().FirstOrDefaultAsync(m => m.MembershipTypeId == membershipTypeId);
        }

        public async Task<MembershipType> AddAsync(MembershipType membershipType)
        {
            _context.MembershipTypes.Add(membershipType);
            await _context.SaveChangesAsync();
            return membershipType;
        }

        public async Task<bool> UpdateAsync(MembershipType membershipType)
        {
            var existing = await _context.MembershipTypes.FirstOrDefaultAsync(m => m.MembershipTypeId == membershipType.MembershipTypeId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(membershipType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int membershipTypeId)
        {
            var existing = await _context.MembershipTypes.FirstOrDefaultAsync(m => m.MembershipTypeId == membershipTypeId);
            if (existing == null)
            {
                return false;
            }

            _context.MembershipTypes.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MembershipTypeNameExistsAsync(string membershipTypeName, int? excludeMembershipTypeId = null)
        {
            return await _context.MembershipTypes.AnyAsync(m => m.MembershipTypeName == membershipTypeName && m.MembershipTypeId != excludeMembershipTypeId);
        }
    }
}
