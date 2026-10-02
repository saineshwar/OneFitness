using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IPaymentTypeService
    {
        Task<IReadOnlyList<PaymentTypeViewModel>> GetAllAsync();
        Task<PaymentTypeViewModel?> GetByIdAsync(int paymentTypeId);
        Task<PaymentTypeServiceResult<PaymentTypeViewModel>> CreateAsync(CreatePaymentTypeViewModel model);
        Task<PaymentTypeServiceResult<PaymentTypeViewModel>> UpdateAsync(int paymentTypeId, UpdatePaymentTypeViewModel model);
        Task<bool> DeleteAsync(int paymentTypeId);
    }

    public class PaymentTypeServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static PaymentTypeServiceResult<T> Success(T data) => new PaymentTypeServiceResult<T> { Succeeded = true, Data = data };
        public static PaymentTypeServiceResult<T> Failure(string error) => new PaymentTypeServiceResult<T> { Succeeded = false, Error = error };
    }
}
