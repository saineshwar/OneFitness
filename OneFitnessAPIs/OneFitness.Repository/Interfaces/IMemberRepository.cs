using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IMemberRepository
    {
        Task<IReadOnlyList<Member>> GetAllAsync();
        Task<(IReadOnlyList<Member> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search);
        Task<Member?> GetByIdAsync(long memberId);
        Task<Member?> GetByMemberNoAsync(string memberNo);
        Task<Member> AddAsync(Member member);
        Task<bool> UpdateAsync(Member member);
        Task<bool> DeleteAsync(long memberId);
        Task<bool> MobileNoExistsAsync(string? mobileNo, long? excludeMemberId = null);
        Task<bool> EmailExistsAsync(string? emailId, long? excludeMemberId = null);
        Task<MemberPhoto?> GetPhotoAsync(long memberId);
        Task SavePhotoAsync(MemberPhoto photo);
        Task<bool> DeletePhotoAsync(long memberId);
    }
}
