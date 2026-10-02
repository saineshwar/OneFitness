using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IWorkOutService
    {
        Task<IReadOnlyList<WorkOutViewModel>> GetAllAsync();
        Task<WorkOutViewModel?> GetByIdAsync(int workOutId);
        Task<WorkOutServiceResult<WorkOutViewModel>> CreateAsync(CreateWorkOutViewModel model);
        Task<WorkOutServiceResult<WorkOutViewModel>> UpdateAsync(int workOutId, UpdateWorkOutViewModel model);
        Task<bool> DeleteAsync(int workOutId);
    }

    public class WorkOutServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static WorkOutServiceResult<T> Success(T data) => new WorkOutServiceResult<T> { Succeeded = true, Data = data };
        public static WorkOutServiceResult<T> Failure(string error) => new WorkOutServiceResult<T> { Succeeded = false, Error = error };
    }
}
