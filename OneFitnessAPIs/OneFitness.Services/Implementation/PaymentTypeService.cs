using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Services.Implementation
{
    public class PaymentTypeService : IPaymentTypeService
    {
        private readonly IPaymentTypeRepository _paymentTypeRepository;

        public PaymentTypeService(IPaymentTypeRepository paymentTypeRepository)
        {
            _paymentTypeRepository = paymentTypeRepository;
        }

        public async Task<IReadOnlyList<PaymentTypeViewModel>> GetAllAsync()
        {
            var paymentTypes = await _paymentTypeRepository.GetAllAsync();
            return paymentTypes.Select(ToViewModel).ToList();
        }

        public async Task<PaymentTypeViewModel?> GetByIdAsync(int paymentTypeId)
        {
            var paymentType = await _paymentTypeRepository.GetByIdAsync(paymentTypeId);
            return paymentType == null ? null : ToViewModel(paymentType);
        }

        public async Task<PaymentTypeServiceResult<PaymentTypeViewModel>> CreateAsync(CreatePaymentTypeViewModel model)
        {
            if (await _paymentTypeRepository.PaymentTypeNameExistsAsync(model.PaymentTypeName))
            {
                return PaymentTypeServiceResult<PaymentTypeViewModel>.Failure("PaymentType already exists.");
            }

            var paymentType = new PaymentType
            {
                PaymentTypeName = model.PaymentTypeName,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _paymentTypeRepository.AddAsync(paymentType);
            return PaymentTypeServiceResult<PaymentTypeViewModel>.Success(ToViewModel(created));
        }

        public async Task<PaymentTypeServiceResult<PaymentTypeViewModel>> UpdateAsync(int paymentTypeId, UpdatePaymentTypeViewModel model)
        {
            var existing = await _paymentTypeRepository.GetByIdAsync(paymentTypeId);
            if (existing == null)
            {
                return PaymentTypeServiceResult<PaymentTypeViewModel>.Failure("PaymentType not found.");
            }

            if (await _paymentTypeRepository.PaymentTypeNameExistsAsync(model.PaymentTypeName, paymentTypeId))
            {
                return PaymentTypeServiceResult<PaymentTypeViewModel>.Failure("PaymentType already exists.");
            }

            existing.PaymentTypeName = model.PaymentTypeName;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _paymentTypeRepository.UpdateAsync(existing);
            return PaymentTypeServiceResult<PaymentTypeViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int paymentTypeId)
        {
            return _paymentTypeRepository.DeleteAsync(paymentTypeId);
        }

        private static PaymentTypeViewModel ToViewModel(PaymentType paymentType)
        {
            return new PaymentTypeViewModel
            {
                PaymentTypeId = paymentType.PaymentTypeId,
                PaymentTypeName = paymentType.PaymentTypeName,
                Status = paymentType.Status,
                CreatedOn = paymentType.CreatedOn,
                ModifiedOn = paymentType.ModifiedOn
            };
        }
    }
}
