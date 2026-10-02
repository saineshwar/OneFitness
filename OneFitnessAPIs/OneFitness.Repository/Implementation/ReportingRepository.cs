using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class ReportingRepository : IReportingRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Member>> GetMembersByCreatedDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return await _context.Members.AsNoTracking()
                .Where(m => m.CreatedOn >= from && m.CreatedOn < toExclusive)
                .ToListAsync();
        }

        public async Task<List<Member>> GetMembersByJoiningDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return await _context.Members.AsNoTracking()
                .Where(m => m.JoiningDate != null && m.JoiningDate >= from && m.JoiningDate < toExclusive)
                .ToListAsync();
        }

        public async Task<List<Refund>> GetRefundsByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return await _context.Refunds.AsNoTracking()
                .Where(r => r.CreatedOn >= from && r.CreatedOn < toExclusive)
                .ToListAsync();
        }

        public async Task<List<ReceiptHistory>> GetReceiptHistoriesByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return await _context.ReceiptHistories.AsNoTracking()
                .Where(r => r.CreatedOn >= from && r.CreatedOn < toExclusive)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return await _context.Payments.AsNoTracking()
                .Where(p => p.CreatedOn >= from && p.CreatedOn < toExclusive)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsWithOutstandingBalanceAsync()
        {
            return await _context.Payments.AsNoTracking()
                .Where(p => p.AmountPaid < p.TotalAmount)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetRenewalPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            var firstPaymentIds = FirstPaymentIdsQuery();
            return await _context.Payments.AsNoTracking()
                .Where(p => !firstPaymentIds.Contains(p.PaymentId) && p.CreatedOn >= from && p.CreatedOn < toExclusive)
                .ToListAsync();
        }

        public Task<int> CountRenewalPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            var firstPaymentIds = FirstPaymentIdsQuery();
            return _context.Payments.AsNoTracking()
                .CountAsync(p => !firstPaymentIds.Contains(p.PaymentId) && p.CreatedOn >= from && p.CreatedOn < toExclusive);
        }

        public async Task<Dictionary<long, Payment>> GetFirstPaymentsByMemberIdsAsync(IReadOnlyCollection<long> memberIds)
        {
            if (memberIds.Count == 0)
            {
                return new Dictionary<long, Payment>();
            }

            var firstPaymentIds = _context.Payments
                .Where(p => memberIds.Contains(p.MemberId))
                .GroupBy(p => p.MemberId)
                .Select(g => g.Min(p => p.PaymentId));

            var payments = await _context.Payments.AsNoTracking()
                .Where(p => firstPaymentIds.Contains(p.PaymentId))
                .ToListAsync();

            return payments.ToDictionary(p => p.MemberId);
        }

        public async Task<Dictionary<long, Payment>> GetLatestPaymentsByMemberIdsAsync(IReadOnlyCollection<long> memberIds)
        {
            if (memberIds.Count == 0)
            {
                return new Dictionary<long, Payment>();
            }

            var latestPaymentIds = _context.Payments
                .Where(p => memberIds.Contains(p.MemberId))
                .GroupBy(p => p.MemberId)
                .Select(g => g.Max(p => p.PaymentId));

            var payments = await _context.Payments.AsNoTracking()
                .Where(p => latestPaymentIds.Contains(p.PaymentId))
                .ToListAsync();

            return payments.ToDictionary(p => p.MemberId);
        }

        public async Task<List<(Member Member, Payment Payment)>> GetMembersWithNextRenewalInRangeAsync(DateTime from, DateTime toExclusive)
        {
            var payments = await LatestPaymentsWithNextRenewalAsync(p => p.NextRenewalDate >= from && p.NextRenewalDate < toExclusive);
            return await JoinMembersAsync(payments);
        }

        public async Task<List<(Member Member, Payment Payment)>> GetActiveMembersWithNextRenewalInRangeAsync(DateTime from, DateTime toExclusive)
        {
            var payments = await LatestPaymentsWithNextRenewalAsync(p => p.NextRenewalDate >= from && p.NextRenewalDate < toExclusive);
            var pairs = await JoinMembersAsync(payments);
            return pairs.Where(x => x.Member.Status).ToList();
        }

        public async Task<List<(Member Member, Payment Payment)>> GetActiveMembersWithNextRenewalBeforeAsync(DateTime beforeExclusive)
        {
            var payments = await LatestPaymentsWithNextRenewalAsync(p => p.NextRenewalDate < beforeExclusive);
            var pairs = await JoinMembersAsync(payments);
            return pairs.Where(x => x.Member.Status).ToList();
        }

        public Task<int> CountMembersByCreatedDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return _context.Members.AsNoTracking()
                .CountAsync(m => m.CreatedOn >= from && m.CreatedOn < toExclusive);
        }

        public Task<int> CountRefundsByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return _context.Refunds.AsNoTracking()
                .CountAsync(r => r.CreatedOn >= from && r.CreatedOn < toExclusive);
        }

        public Task<int> CountAllMembersAsync()
        {
            return _context.Members.AsNoTracking().CountAsync();
        }

        public Task<int> CountActiveMembersAsync()
        {
            return _context.Members.AsNoTracking().CountAsync(m => m.Status);
        }

        public Task<decimal> SumReceiptTotalByDateRangeAsync(DateTime from, DateTime toExclusive)
        {
            return _context.ReceiptHistories.AsNoTracking()
                .Where(r => r.CreatedOn >= from && r.CreatedOn < toExclusive)
                .SumAsync(r => r.TotalAmount ?? 0);
        }

        private IQueryable<long> FirstPaymentIdsQuery()
        {
            return _context.Payments.GroupBy(p => p.MemberId).Select(g => g.Min(p => p.PaymentId));
        }

        private async Task<List<Payment>> LatestPaymentsWithNextRenewalAsync(System.Linq.Expressions.Expression<Func<Payment, bool>> renewalFilter)
        {
            var latestPaymentIds = _context.Payments.GroupBy(p => p.MemberId).Select(g => g.Max(p => p.PaymentId));
            return await _context.Payments.AsNoTracking()
                .Where(p => latestPaymentIds.Contains(p.PaymentId))
                .Where(renewalFilter)
                .ToListAsync();
        }

        private async Task<List<(Member Member, Payment Payment)>> JoinMembersAsync(List<Payment> payments)
        {
            if (payments.Count == 0)
            {
                return new List<(Member, Payment)>();
            }

            var memberIds = payments.Select(p => p.MemberId).Distinct().ToList();
            var members = await _context.Members.AsNoTracking()
                .Where(m => memberIds.Contains(m.MemberId))
                .ToDictionaryAsync(m => m.MemberId);

            return payments
                .Where(p => members.ContainsKey(p.MemberId))
                .Select(p => (members[p.MemberId], p))
                .ToList();
        }
    }
}
