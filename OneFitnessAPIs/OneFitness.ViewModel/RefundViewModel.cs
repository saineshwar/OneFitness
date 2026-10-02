using System;

namespace OneFitness.ViewModel
{
    public class RefundViewModel
    {
        public int RefundId { get; set; }
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string MemberFullName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
