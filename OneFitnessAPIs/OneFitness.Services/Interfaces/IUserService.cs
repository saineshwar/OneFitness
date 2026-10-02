using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserViewModel>> GetAllAsync();
        Task<UserViewModel?> GetByIdAsync(int userId);
        Task<UserServiceResult<UserViewModel>> CreateAsync(CreateUserViewModel model);
        Task<UserServiceResult<UserViewModel>> UpdateAsync(int userId, UpdateUserViewModel model);
        Task<UserServiceResult<UserViewModel>> ResetPasswordAsync(int userId, ResetPasswordViewModel model);
        Task<bool> DeleteAsync(int userId);
        Task<UserServiceResult<LoginResultViewModel>> LoginAsync(LoginViewModel model);
    }

    public class UserServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static UserServiceResult<T> Success(T data) => new UserServiceResult<T> { Succeeded = true, Data = data };
        public static UserServiceResult<T> Failure(string error) => new UserServiceResult<T> { Succeeded = false, Error = error };
    }
}
