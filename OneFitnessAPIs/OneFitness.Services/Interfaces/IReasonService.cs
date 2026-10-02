using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IReasonService
    {
        Task<IReadOnlyList<ReasonViewModel>> GetAllAsync();
        Task<ReasonViewModel?> GetByIdAsync(int reasonId);
        Task<ReasonServiceResult<ReasonViewModel>> CreateAsync(CreateReasonViewModel model);
        Task<ReasonServiceResult<ReasonViewModel>> UpdateAsync(int reasonId, UpdateReasonViewModel model);
        Task<bool> DeleteAsync(int reasonId);
    }

    public class ReasonServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static ReasonServiceResult<T> Success(T data) => new ReasonServiceResult<T> { Succeeded = true, Data = data };
        public static ReasonServiceResult<T> Failure(string error) => new ReasonServiceResult<T> { Succeeded = false, Error = error };
    }
}
