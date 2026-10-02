using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.Services.Interfaces
{
    public interface IReportingService
    {
        Task<YearwiseReportRowViewModel> GetYearwiseReportAsync(int fiscalYear);
        Task<IReadOnlyList<MonthlyReportMemberViewModel>> GetMonthwiseReportAsync(int year, int month);
        Task<IReadOnlyList<RenewalReportRowViewModel>> GetRenewalReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<MemberJoinedRowViewModel>> GetJoinedReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<RefundReportRowViewModel>> GetRefundReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<IncomeCreditDebitRowViewModel>> GetIncomeCreditDebitReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<TaxSummaryRowViewModel>> GetTaxSummaryReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<PaymentTypeCollectionRowViewModel>> GetPaymentTypeCollectionReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<StaffCollectionRowViewModel>> GetStaffCollectionReportAsync(DateRangeReportRequestViewModel request);
        Task<IReadOnlyList<RenewalStatusRowViewModel>> GetRenewalStatusReportAsync(int daysAhead);
        Task<IReadOnlyList<OutstandingBalanceRowViewModel>> GetOutstandingBalancesReportAsync();
        Task<byte[]> GetTallyExportXmlAsync(DateRangeReportRequestViewModel request);
        Task<DashboardSummaryViewModel> GetDashboardSummaryAsync();
    }
}
