using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IMembershipTypeRepository
    {
        Task<IReadOnlyList<MembershipType>> GetAllAsync();
        Task<MembershipType?> GetByIdAsync(int membershipTypeId);
        Task<MembershipType> AddAsync(MembershipType membershipType);
        Task<bool> UpdateAsync(MembershipType membershipType);
        Task<bool> DeleteAsync(int membershipTypeId);
        Task<bool> MembershipTypeNameExistsAsync(string membershipTypeName, int? excludeMembershipTypeId = null);
    }
}
