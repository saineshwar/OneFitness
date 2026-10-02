using System;

namespace OneFitness.ViewModel
{
    public class MemberJoinedRowViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string? WorkOutName { get; set; }
        public string? MembershipTypeName { get; set; }
        public string? InstallmentName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public bool Status { get; set; }
    }
}
