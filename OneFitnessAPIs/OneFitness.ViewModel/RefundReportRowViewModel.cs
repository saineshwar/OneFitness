using System;

namespace OneFitness.ViewModel
{
    public class RefundReportRowViewModel
    {
        public int RefundId { get; set; }
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string? InstallmentName { get; set; }
        public string? MembershipTypeName { get; set; }
        public string? WorkOutName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public decimal? SubscriptionAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundedDate { get; set; }
    }
}
