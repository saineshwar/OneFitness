using System;

namespace OneFitness.ViewModel
{
    public class RenewalStatusRowViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string? WorkOutName { get; set; }
        public string? MembershipTypeName { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public int DaysRemaining { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
    }
}
