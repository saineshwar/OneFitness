using System;

namespace OneFitness.ViewModel
{
    public class MembershipTypeViewModel
    {
        public int MembershipTypeId { get; set; }
        public string MembershipTypeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int? InstallmentId { get; set; }
        public int? WorkOutId { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
