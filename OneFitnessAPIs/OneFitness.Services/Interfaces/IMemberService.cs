using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IReadOnlyList<MemberViewModel>> GetAllAsync();
        Task<PagedResultViewModel<MemberViewModel>> GetPagedAsync(int page, int pageSize, string? search);
        Task<MemberViewModel?> GetByIdAsync(long memberId);
        Task<MemberServiceResult<MemberViewModel>> CreateAsync(CreateMemberViewModel model);
        Task<MemberServiceResult<MemberViewModel>> UpdateAsync(long memberId, UpdateMemberViewModel model);
        Task<bool> DeleteAsync(long memberId);
        Task<bool> ActivateAsync(long memberId);
        Task<bool> DeactivateAsync(long memberId);
        Task<MemberPhotoViewModel?> GetPhotoAsync(long memberId);
        Task<MemberServiceResult<MemberPhotoViewModel>> SavePhotoAsync(long memberId, MemberPhotoViewModel model);
        Task<bool> DeletePhotoAsync(long memberId);
    }

    public class MemberServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static MemberServiceResult<T> Success(T data) => new MemberServiceResult<T> { Succeeded = true, Data = data };
        public static MemberServiceResult<T> Failure(string error) => new MemberServiceResult<T> { Succeeded = false, Error = error };
    }
}
