using System;

namespace OneFitness.ViewModel
{
    public class PaymentViewModel
    {
        public long PaymentId { get; set; }
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string MemberFullName { get; set; } = string.Empty;
        public int WorkOutId { get; set; }
        public int MembershipTypeId { get; set; }
        public int InstallmentId { get; set; }
        public int PaymentTypeId { get; set; }
        public int TaxId { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxPercentageAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public long InvoiceNo { get; set; }
        public DateTime PaymentFromDate { get; set; }
        public DateTime NextRenewalDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
