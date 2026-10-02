using System;

namespace OneFitness.ViewModel
{
    public class OutstandingBalanceRowViewModel
    {
        public long PaymentId { get; set; }
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string? MembershipTypeName { get; set; }
        public string? WorkOutName { get; set; }
        public string? InstallmentName { get; set; }
        public long InvoiceNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceDue { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime NextRenewalDate { get; set; }
    }
}
