using System;

namespace OneFitness.ViewModel
{
    public class MonthlyReportMemberViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool Status { get; set; }
    }
}
