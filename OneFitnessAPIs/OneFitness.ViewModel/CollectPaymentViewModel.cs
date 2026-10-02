using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CollectPaymentViewModel
    {
        [Required]
        public decimal Amount { get; set; }
    }
}
