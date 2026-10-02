using System;

namespace OneFitness.ViewModel
{
    public class RenewalReportRowViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? InstallmentName { get; set; }
        public string? MembershipTypeName { get; set; }
        public string? WorkOutName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
