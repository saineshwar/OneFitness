using OneFitness.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Repository.Interfaces
{
    public interface IReportingRepository
    {
        Task<List<Member>> GetMembersByCreatedDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<Member>> GetMembersByJoiningDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<Refund>> GetRefundsByDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<ReceiptHistory>> GetReceiptHistoriesByDateRangeAsync(DateTime from, DateTime toExclusive);

        Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<Payment>> GetPaymentsWithOutstandingBalanceAsync();
        Task<List<Payment>> GetRenewalPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<int> CountRenewalPaymentsByDateRangeAsync(DateTime from, DateTime toExclusive);

        Task<Dictionary<long, Payment>> GetFirstPaymentsByMemberIdsAsync(IReadOnlyCollection<long> memberIds);
        Task<Dictionary<long, Payment>> GetLatestPaymentsByMemberIdsAsync(IReadOnlyCollection<long> memberIds);

        Task<List<(Member Member, Payment Payment)>> GetMembersWithNextRenewalInRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<(Member Member, Payment Payment)>> GetActiveMembersWithNextRenewalInRangeAsync(DateTime from, DateTime toExclusive);
        Task<List<(Member Member, Payment Payment)>> GetActiveMembersWithNextRenewalBeforeAsync(DateTime beforeExclusive);

        Task<int> CountMembersByCreatedDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<int> CountRefundsByDateRangeAsync(DateTime from, DateTime toExclusive);
        Task<int> CountAllMembersAsync();
        Task<int> CountActiveMembersAsync();
        Task<decimal> SumReceiptTotalByDateRangeAsync(DateTime from, DateTime toExclusive);
    }
}
