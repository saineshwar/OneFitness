using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreatePaymentTypeViewModel
    {
        [Required, MaxLength(50)]
        public string PaymentTypeName { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
