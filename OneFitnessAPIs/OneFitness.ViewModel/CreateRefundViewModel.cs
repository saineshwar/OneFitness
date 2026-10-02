using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateRefundViewModel
    {
        [Required]
        public long MemberId { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
