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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipTypeRepository _membershipTypeRepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IInstallmentRepository _installmentRepository;
        private readonly IReceiptHistoryRepository _receiptHistoryRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IMemberRepository memberRepository,
            IMembershipTypeRepository membershipTypeRepository,
            ITaxMasterRepository taxMasterRepository,
            IInstallmentRepository installmentRepository,
            IReceiptHistoryRepository receiptHistoryRepository)
        {
            _paymentRepository = paymentRepository;
            _memberRepository = memberRepository;
            _membershipTypeRepository = membershipTypeRepository;
            _taxMasterRepository = taxMasterRepository;
            _installmentRepository = installmentRepository;
            _receiptHistoryRepository = receiptHistoryRepository;
        }

        public async Task<PagedResultViewModel<PaymentViewModel>> GetPagedAsync(int page, int pageSize, string? search)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, totalCount) = await _paymentRepository.GetPagedAsync(page, pageSize, search);
            var result = new List<PaymentViewModel>();
            foreach (var payment in items)
            {
                result.Add(await ToViewModelAsync(payment));
            }

            return new PagedResultViewModel<PaymentViewModel>
            {
                Items = result,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyList<PaymentViewModel>> GetByMemberIdAsync(long memberId)
        {
            var payments = await _paymentRepository.GetByMemberIdAsync(memberId);
            var result = new List<PaymentViewModel>();
            foreach (var payment in payments)
            {
                result.Add(await ToViewModelAsync(payment));
            }

            return result;
        }

        public async Task<PaymentViewModel?> GetLatestByMemberIdAsync(long memberId)
        {
            var payment = await _paymentRepository.GetLatestByMemberIdAsync(memberId);
            return payment == null ? null : await ToViewModelAsync(payment);
        }

        public async Task<PaymentViewModel?> GetByIdAsync(long paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            return payment == null ? null : await ToViewModelAsync(payment);
        }

        public async Task<PaymentServiceResult<PaymentAmountCalculationViewModel>> CalculateAmountAsync(CalculatePaymentAmountViewModel model)
        {
            var calculation = await CalculateAmountInternalAsync(model.MembershipTypeId, model.TaxId);
            if (calculation == null)
            {
                return PaymentServiceResult<PaymentAmountCalculationViewModel>.Failure("MembershipType or TaxType not found.");
            }

            return PaymentServiceResult<PaymentAmountCalculationViewModel>.Success(calculation);
        }

        public async Task<PaymentServiceResult<PaymentViewModel>> CreatePaymentAsync(CreatePaymentViewModel model, int? createdBy)
        {
            var member = await _memberRepository.GetByIdAsync(model.MemberId);
            if (member == null)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Member not found.");
            }

            var calculation = await CalculateAmountInternalAsync(model.MembershipTypeId, model.TaxId);
            if (calculation == null)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("MembershipType or TaxType not found.");
            }

            var installment = await _installmentRepository.GetByIdAsync(model.InstallmentId);
            if (installment == null)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Installment not found.");
            }

            var amountPaid = model.AmountPaid ?? calculation.TotalAmount;
            if (amountPaid <= 0)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Amount paid must be greater than zero.");
            }

            if (amountPaid > calculation.TotalAmount)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Amount paid cannot exceed the total amount due.");
            }

            var previousPayment = await _paymentRepository.GetLatestByMemberIdAsync(model.MemberId);
            var paymentFromDate = previousPayment?.NextRenewalDate ?? member.JoiningDate ?? DateTime.UtcNow;
            var nextRenewalDate = paymentFromDate.AddMonths(installment.InstallmentMonths ?? 0);

            var payment = new Payment
            {
                MemberId = model.MemberId,
                WorkOutId = model.WorkOutId,
                MembershipTypeId = model.MembershipTypeId,
                InstallmentId = model.InstallmentId,
                PaymentTypeId = model.PaymentTypeId,
                TaxId = model.TaxId,
                Amount = calculation.Amount,
                TaxPercentage = calculation.TaxPercentage,
                TaxPercentageAmount = calculation.TaxPercentageAmount,
                TotalAmount = calculation.TotalAmount,
                AmountPaid = amountPaid,
                InvoiceNo = await _paymentRepository.GetNextInvoiceNoAsync(),
                PaymentFromDate = paymentFromDate,
                NextRenewalDate = nextRenewalDate,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var created = await _paymentRepository.AddAsync(payment);

            await _receiptHistoryRepository.AddAsync(BuildReceiptHistorySnapshot(created, amountPaid, member.MemberNo, createdBy, created.CreatedOn));

            return PaymentServiceResult<PaymentViewModel>.Success(await ToViewModelAsync(created));
        }

        public async Task<PaymentServiceResult<PaymentViewModel>> CollectBalanceAsync(long paymentId, CollectPaymentViewModel model, int? createdBy)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Payment not found.");
            }

            if (model.Amount <= 0)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Amount must be greater than zero.");
            }

            var balanceDue = payment.TotalAmount - payment.AmountPaid;
            if (model.Amount > balanceDue)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Amount exceeds the remaining balance.");
            }

            var member = await _memberRepository.GetByIdAsync(payment.MemberId);
            if (member == null)
            {
                return PaymentServiceResult<PaymentViewModel>.Failure("Member not found.");
            }

            payment.AmountPaid += model.Amount;
            await _paymentRepository.UpdateAsync(payment);

            var collectedOn = DateTime.UtcNow;
            await _receiptHistoryRepository.AddAsync(BuildReceiptHistorySnapshot(payment, model.Amount, member.MemberNo, createdBy, collectedOn));

            return PaymentServiceResult<PaymentViewModel>.Success(await ToViewModelAsync(payment));
        }

        private static ReceiptHistory BuildReceiptHistorySnapshot(Payment payment, decimal amountCollected, string memberNo, int? createdBy, DateTime createdOn)
        {
            var fraction = payment.TotalAmount == 0 ? 1m : amountCollected / payment.TotalAmount;

            return new ReceiptHistory
            {
                InvoiceNo = payment.InvoiceNo,
                MemberNo = memberNo,
                CreatedOn = createdOn,
                CreatedBy = createdBy,
                WorkOutId = payment.WorkOutId,
                MembershipTypeId = payment.MembershipTypeId,
                InstallmentId = payment.InstallmentId,
                PaymentTypeId = payment.PaymentTypeId,
                TaxId = payment.TaxId,
                Amount = Math.Round(payment.Amount * fraction, 2),
                TaxPercentage = payment.TaxPercentage,
                TaxPercentageAmount = Math.Round(payment.TaxPercentageAmount * fraction, 2),
                TotalAmount = amountCollected,
                PaymentFromDate = payment.PaymentFromDate,
                NextRenewalDate = payment.NextRenewalDate
            };
        }

        private async Task<PaymentAmountCalculationViewModel?> CalculateAmountInternalAsync(int membershipTypeId, int taxId)
        {
            var membershipType = await _membershipTypeRepository.GetByIdAsync(membershipTypeId);
            var taxMaster = await _taxMasterRepository.GetByIdAsync(taxId);
            if (membershipType == null || taxMaster == null)
            {
                return null;
            }

            var taxPercentageAmount = membershipType.Amount * (taxMaster.TaxRate / 100);
            var totalAmount = membershipType.Amount + taxPercentageAmount;

            return new PaymentAmountCalculationViewModel
            {
                Amount = membershipType.Amount,
                TaxPercentage = taxMaster.TaxRate,
                TaxPercentageAmount = taxPercentageAmount,
                TotalAmount = totalAmount
            };
        }

        private async Task<PaymentViewModel> ToViewModelAsync(Payment payment)
        {
            var member = await _memberRepository.GetByIdAsync(payment.MemberId);

            return new PaymentViewModel
            {
                PaymentId = payment.PaymentId,
                MemberId = payment.MemberId,
                MemberNo = member?.MemberNo ?? string.Empty,
                MemberFullName = member == null ? string.Empty : $"{member.FirstName} {member.MiddleName} {member.LastName}".Replace("  ", " ").Trim(),
                WorkOutId = payment.WorkOutId,
                MembershipTypeId = payment.MembershipTypeId,
                InstallmentId = payment.InstallmentId,
                PaymentTypeId = payment.PaymentTypeId,
                TaxId = payment.TaxId,
                Amount = payment.Amount,
                TaxPercentage = payment.TaxPercentage,
                TaxPercentageAmount = payment.TaxPercentageAmount,
                TotalAmount = payment.TotalAmount,
                AmountPaid = payment.AmountPaid,
                BalanceDue = payment.TotalAmount - payment.AmountPaid,
                InvoiceNo = payment.InvoiceNo,
                PaymentFromDate = payment.PaymentFromDate,
                NextRenewalDate = payment.NextRenewalDate,
                CreatedOn = payment.CreatedOn
            };
        }
    }
}
