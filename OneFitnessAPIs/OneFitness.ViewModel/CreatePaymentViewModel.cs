using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreatePaymentViewModel
    {
        [Required]
        public long MemberId { get; set; }

        [Required]
        public int WorkOutId { get; set; }

        [Required]
        public int MembershipTypeId { get; set; }

        [Required]
        public int InstallmentId { get; set; }

        [Required]
        public int PaymentTypeId { get; set; }

        [Required]
        public int TaxId { get; set; }

        /// <summary>Amount being collected now. Omit or send the full total due for a full payment; send less for a partial payment.</summary>
        public decimal? AmountPaid { get; set; }
    }
}
