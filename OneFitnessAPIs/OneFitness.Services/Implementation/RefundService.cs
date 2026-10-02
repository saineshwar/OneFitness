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
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IPaymentRepository _paymentRepository;

        public RefundService(IRefundRepository refundRepository, IMemberRepository memberRepository, IPaymentRepository paymentRepository)
        {
            _refundRepository = refundRepository;
            _memberRepository = memberRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IReadOnlyList<RefundViewModel>> GetAllAsync()
        {
            var refunds = await _refundRepository.GetAllAsync();
            var result = new List<RefundViewModel>();
            foreach (var refund in refunds)
            {
                result.Add(await ToViewModelAsync(refund));
            }

            return result;
        }

        public async Task<PagedResultViewModel<RefundViewModel>> GetPagedAsync(int page, int pageSize, string? search)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, totalCount) = await _refundRepository.GetPagedAsync(page, pageSize, search);
            var result = new List<RefundViewModel>();
            foreach (var refund in items)
            {
                result.Add(await ToViewModelAsync(refund));
            }

            return new PagedResultViewModel<RefundViewModel>
            {
                Items = result,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<RefundViewModel?> GetByIdAsync(int refundId)
        {
            var refund = await _refundRepository.GetByIdAsync(refundId);
            return refund == null ? null : await ToViewModelAsync(refund);
        }

        public async Task<RefundServiceResult<RefundViewModel>> CreateAsync(CreateRefundViewModel model, int? createdBy)
        {
            var member = await _memberRepository.GetByIdAsync(model.MemberId);
            if (member == null)
            {
                return RefundServiceResult<RefundViewModel>.Failure("Member not found.");
            }

            if (await _refundRepository.RefundExistsForMemberAsync(model.MemberId))
            {
                return RefundServiceResult<RefundViewModel>.Failure("A refund already exists for this member.");
            }

            var latestPayment = await _paymentRepository.GetLatestByMemberIdAsync(model.MemberId);
            if (latestPayment == null)
            {
                return RefundServiceResult<RefundViewModel>.Failure("No payment found for this member.");
            }

            if (model.Amount > latestPayment.AmountPaid)
            {
                return RefundServiceResult<RefundViewModel>.Failure("Refund amount cannot exceed the amount actually paid.");
            }

            if (DateTime.UtcNow > latestPayment.NextRenewalDate)
            {
                return RefundServiceResult<RefundViewModel>.Failure("Refund is not allowed after the membership has expired.");
            }

            var refund = new Refund
            {
                MemberId = model.MemberId,
                Amount = model.Amount,
                Status = true,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            var created = await _refundRepository.AddAsync(refund);
            return RefundServiceResult<RefundViewModel>.Success(await ToViewModelAsync(created));
        }

        public Task<bool> DeleteAsync(int refundId)
        {
            return _refundRepository.DeleteAsync(refundId);
        }

        private async Task<RefundViewModel> ToViewModelAsync(Refund refund)
        {
            var member = await _memberRepository.GetByIdAsync(refund.MemberId);

            return new RefundViewModel
            {
                RefundId = refund.RefundId,
                MemberId = refund.MemberId,
                MemberNo = member?.MemberNo ?? string.Empty,
                MemberFullName = member == null ? string.Empty : $"{member.FirstName} {member.MiddleName} {member.LastName}".Replace("  ", " ").Trim(),
                Amount = refund.Amount,
                Status = refund.Status,
                CreatedOn = refund.CreatedOn,
                ModifiedOn = refund.ModifiedOn
            };
        }
    }
}
