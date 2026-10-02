using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PagedResultViewModel<PaymentViewModel>> GetPagedAsync(int page, int pageSize, string? search);
        Task<IReadOnlyList<PaymentViewModel>> GetByMemberIdAsync(long memberId);
        Task<PaymentViewModel?> GetLatestByMemberIdAsync(long memberId);
        Task<PaymentViewModel?> GetByIdAsync(long paymentId);
        Task<PaymentServiceResult<PaymentAmountCalculationViewModel>> CalculateAmountAsync(CalculatePaymentAmountViewModel model);
        Task<PaymentServiceResult<PaymentViewModel>> CreatePaymentAsync(CreatePaymentViewModel model, int? createdBy);
        Task<PaymentServiceResult<PaymentViewModel>> CollectBalanceAsync(long paymentId, CollectPaymentViewModel model, int? createdBy);
    }

    public class PaymentServiceResult<T>
    {
        public bool Succeeded { get; private set; }
        public string? Error { get; private set; }
        public T? Data { get; private set; }

        public static PaymentServiceResult<T> Success(T data) => new PaymentServiceResult<T> { Succeeded = true, Data = data };
        public static PaymentServiceResult<T> Failure(string error) => new PaymentServiceResult<T> { Succeeded = false, Error = error };
    }
}
