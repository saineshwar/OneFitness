using OneFitness.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IPaymentTypeRepository
    {
        Task<IReadOnlyList<PaymentType>> GetAllAsync();
        Task<PaymentType?> GetByIdAsync(int paymentTypeId);
        Task<PaymentType> AddAsync(PaymentType paymentType);
        Task<bool> UpdateAsync(PaymentType paymentType);
        Task<bool> DeleteAsync(int paymentTypeId);
        Task<bool> PaymentTypeNameExistsAsync(string paymentTypeName, int? excludePaymentTypeId = null);
    }
}
