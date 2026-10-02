using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public long PaymentId { get; set; }

        public long MemberId { get; set; }

        public int WorkOutId { get; set; }

        public int MembershipTypeId { get; set; }

        public int InstallmentId { get; set; }

        public int PaymentTypeId { get; set; }

        public int TaxId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxPercentage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxPercentageAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AmountPaid { get; set; }

        public long InvoiceNo { get; set; }

        public DateTime PaymentFromDate { get; set; }

        public DateTime NextRenewalDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int? CreatedBy { get; set; }
    }
}
