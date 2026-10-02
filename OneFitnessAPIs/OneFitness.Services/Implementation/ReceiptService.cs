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
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptHistoryRepository _receiptHistoryRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWorkOutRepository _workOutRepository;
        private readonly IMembershipTypeRepository _membershipTypeRepository;
        private readonly IInstallmentRepository _installmentRepository;
        private readonly IPaymentTypeRepository _paymentTypeRepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IGeneralSettingsRepository _generalSettingsRepository;

        public ReceiptService(
            IReceiptHistoryRepository receiptHistoryRepository,
            IMemberRepository memberRepository,
            IPaymentRepository paymentRepository,
            IWorkOutRepository workOutRepository,
            IMembershipTypeRepository membershipTypeRepository,
            IInstallmentRepository installmentRepository,
            IPaymentTypeRepository paymentTypeRepository,
            ITaxMasterRepository taxMasterRepository,
            IGeneralSettingsRepository generalSettingsRepository)
        {
            _receiptHistoryRepository = receiptHistoryRepository;
            _memberRepository = memberRepository;
            _paymentRepository = paymentRepository;
            _workOutRepository = workOutRepository;
            _membershipTypeRepository = membershipTypeRepository;
            _installmentRepository = installmentRepository;
            _paymentTypeRepository = paymentTypeRepository;
            _taxMasterRepository = taxMasterRepository;
            _generalSettingsRepository = generalSettingsRepository;
        }

        public async Task<IReadOnlyList<ReceiptHistoryViewModel>> GetAllHistoryAsync()
        {
            var history = await _receiptHistoryRepository.GetAllAsync();
            return history.Select(ToViewModel).ToList();
        }

        public async Task<PagedResultViewModel<ReceiptHistoryViewModel>> GetPagedHistoryAsync(int page, int pageSize, string? search)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, totalCount) = await _receiptHistoryRepository.GetPagedAsync(page, pageSize, search);
            return new PagedResultViewModel<ReceiptHistoryViewModel>
            {
                Items = items.Select(ToViewModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ReceiptServiceResult<ReceiptViewModel>> GenerateAsync(long memberId, int? createdBy)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
            {
                return ReceiptServiceResult<ReceiptViewModel>.Failure("Member not found.");
            }

            var payment = await _paymentRepository.GetLatestByMemberIdAsync(memberId);
            if (payment == null)
            {
                return ReceiptServiceResult<ReceiptViewModel>.Failure("No payment found for this member.");
            }

            var workOut = await _workOutRepository.GetByIdAsync(payment.WorkOutId);
            var membershipType = await _membershipTypeRepository.GetByIdAsync(payment.MembershipTypeId);
            var installment = await _installmentRepository.GetByIdAsync(payment.InstallmentId);
            var paymentType = await _paymentTypeRepository.GetByIdAsync(payment.PaymentTypeId);
            var taxMaster = await _taxMasterRepository.GetByIdAsync(payment.TaxId);
            var generalSettings = await _generalSettingsRepository.GetActiveAsync();

            var receipt = new ReceiptViewModel
            {
                MemberId = member.MemberId,
                MemberNo = member.MemberNo,
                FirstName = member.FirstName,
                MiddleName = member.MiddleName,
                LastName = member.LastName,
                PaymentFromDate = payment.PaymentFromDate,
                NextRenewalDate = payment.NextRenewalDate,
                InstallmentName = installment?.InstallmentName,
                MembershipTypeName = membershipType?.MembershipTypeName,
                WorkOutName = workOut?.WorkOutName,
                PaymentTypeName = paymentType?.PaymentTypeName,
                TaxRate = taxMaster?.TaxRate,
                TaxType = taxMaster?.TaxType,
                IdentificationNo = taxMaster?.IdentificationNo,
                Amount = payment.Amount,
                TaxPercentage = payment.TaxPercentage,
                TaxPercentageAmount = payment.TaxPercentageAmount,
                TotalAmount = payment.TotalAmount,
                AmountPaid = payment.AmountPaid,
                BalanceDue = payment.TotalAmount - payment.AmountPaid,
                InvoiceNo = payment.InvoiceNo,
                InvoiceDate = DateTime.UtcNow,
                CompanyName = generalSettings?.Name,
                CompanyAddress = generalSettings?.Address,
                CompanyLogoPath = generalSettings?.Logopath,
                CompanySupportEmailId = generalSettings?.SupportEmailId,
                CompanyTelephoneNo = generalSettings?.TelephoneNo
            };

            var receiptHistory = new ReceiptHistory
            {
                InvoiceNo = payment.InvoiceNo,
                MemberNo = member.MemberNo,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = createdBy,
                WorkOutId = payment.WorkOutId,
                MembershipTypeId = payment.MembershipTypeId,
                InstallmentId = payment.InstallmentId,
                PaymentTypeId = payment.PaymentTypeId,
                TaxId = payment.TaxId,
                Amount = payment.Amount,
                TaxPercentage = payment.TaxPercentage,
                TaxPercentageAmount = payment.TaxPercentageAmount,
                TotalAmount = payment.TotalAmount,
                PaymentFromDate = payment.PaymentFromDate,
                NextRenewalDate = payment.NextRenewalDate
            };
            await _receiptHistoryRepository.AddAsync(receiptHistory);

            return ReceiptServiceResult<ReceiptViewModel>.Success(receipt);
        }

        public async Task<ReceiptServiceResult<ReceiptViewModel>> GetHistoryDetailAsync(long receiptHistoryId)
        {
            var history = await _receiptHistoryRepository.GetByIdAsync(receiptHistoryId);
            if (history == null)
            {
                return ReceiptServiceResult<ReceiptViewModel>.Failure("Receipt history entry not found.");
            }

            var member = await _memberRepository.GetByMemberNoAsync(history.MemberNo);
            if (member == null)
            {
                return ReceiptServiceResult<ReceiptViewModel>.Failure("Member not found.");
            }

            var workOut = history.WorkOutId.HasValue ? await _workOutRepository.GetByIdAsync(history.WorkOutId.Value) : null;
            var membershipType = history.MembershipTypeId.HasValue ? await _membershipTypeRepository.GetByIdAsync(history.MembershipTypeId.Value) : null;
            var installment = history.InstallmentId.HasValue ? await _installmentRepository.GetByIdAsync(history.InstallmentId.Value) : null;
            var paymentType = history.PaymentTypeId.HasValue ? await _paymentTypeRepository.GetByIdAsync(history.PaymentTypeId.Value) : null;
            var taxMaster = history.TaxId.HasValue ? await _taxMasterRepository.GetByIdAsync(history.TaxId.Value) : null;
            var generalSettings = await _generalSettingsRepository.GetActiveAsync();

            var receipt = new ReceiptViewModel
            {
                MemberId = member.MemberId,
                MemberNo = member.MemberNo,
                FirstName = member.FirstName,
                MiddleName = member.MiddleName,
                LastName = member.LastName,
                PaymentFromDate = history.PaymentFromDate,
                NextRenewalDate = history.NextRenewalDate,
                InstallmentName = installment?.InstallmentName,
                MembershipTypeName = membershipType?.MembershipTypeName,
                WorkOutName = workOut?.WorkOutName,
                PaymentTypeName = paymentType?.PaymentTypeName,
                TaxRate = taxMaster?.TaxRate,
                TaxType = taxMaster?.TaxType,
                IdentificationNo = taxMaster?.IdentificationNo,
                Amount = history.Amount,
                TaxPercentage = history.TaxPercentage,
                TaxPercentageAmount = history.TaxPercentageAmount,
                TotalAmount = history.TotalAmount,
                InvoiceNo = history.InvoiceNo,
                InvoiceDate = history.CreatedOn,
                CompanyName = generalSettings?.Name,
                CompanyAddress = generalSettings?.Address,
                CompanyLogoPath = generalSettings?.Logopath,
                CompanySupportEmailId = generalSettings?.SupportEmailId,
                CompanyTelephoneNo = generalSettings?.TelephoneNo
            };

            return ReceiptServiceResult<ReceiptViewModel>.Success(receipt);
        }

        private static ReceiptHistoryViewModel ToViewModel(ReceiptHistory receiptHistory)
        {
            return new ReceiptHistoryViewModel
            {
                ReceiptHistoryId = receiptHistory.ReceiptHistoryId,
                InvoiceNo = receiptHistory.InvoiceNo,
                MemberNo = receiptHistory.MemberNo,
                CreatedOn = receiptHistory.CreatedOn,
                CreatedBy = receiptHistory.CreatedBy
            };
        }
    }
}
