using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class UpdateReasonViewModel
    {
        [Required, MaxLength(50)]
        public string ReasonName { get; set; } = string.Empty;
    }
}
