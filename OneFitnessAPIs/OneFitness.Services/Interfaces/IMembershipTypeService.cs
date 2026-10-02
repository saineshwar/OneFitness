using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IMembershipTypeService
    {
        Task<IReadOnlyList<MembershipTypeViewModel>> GetAllAsync();
        Task<MembershipTypeViewModel?> GetByIdAsync(int membershipTypeId);
        Task<MembershipTypeServiceResult<MembershipTypeViewModel>> CreateAsync(CreateMembershipTypeViewModel model);
        Task<MembershipTypeServiceResult<MembershipTypeViewModel>> UpdateAsync(int membershipTypeId, UpdateMembershipTypeViewModel model);
        Task<bool> DeleteAsync(int membershipTypeId);
    }

    public class MembershipTypeServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static MembershipTypeServiceResult<T> Success(T data) => new MembershipTypeServiceResult<T> { Succeeded = true, Data = data };
        public static MembershipTypeServiceResult<T> Failure(string error) => new MembershipTypeServiceResult<T> { Succeeded = false, Error = error };
    }
}
