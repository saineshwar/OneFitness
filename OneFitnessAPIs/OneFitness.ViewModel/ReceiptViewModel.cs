using System;

namespace OneFitness.ViewModel
{
    public class ReceiptViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? PaymentFromDate { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public string? InstallmentName { get; set; }
        public string? MembershipTypeName { get; set; }
        public string? WorkOutName { get; set; }
        public string? PaymentTypeName { get; set; }
        public decimal? TaxRate { get; set; }
        public string? TaxType { get; set; }
        public string? IdentificationNo { get; set; }
        public decimal? Amount { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxPercentageAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? BalanceDue { get; set; }
        public long? InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? CompanyLogoPath { get; set; }
        public string? CompanySupportEmailId { get; set; }
        public string? CompanyTelephoneNo { get; set; }
    }
}
