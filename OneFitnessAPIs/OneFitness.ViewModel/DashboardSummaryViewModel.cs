using System.Collections.Generic;

namespace OneFitness.ViewModel
{
    public class DashboardSummaryViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int NewRegistrationsThisMonth { get; set; }
        public int NewRegistrationsToday { get; set; }
        public int RenewedThisMonth { get; set; }
        public int RefundsThisMonth { get; set; }
        public int EnquiryCount { get; set; }
        public YearwiseChartViewModel YearwiseNewChart { get; set; } = new();
        public YearwiseChartViewModel YearwiseRenewedChart { get; set; } = new();
        public List<TopMembershipTypeViewModel> TopMembershipTypes { get; set; } = new();
    }
}
