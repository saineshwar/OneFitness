using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateReasonViewModel
    {
        [Required, MaxLength(50)]
        public string ReasonName { get; set; } = string.Empty;
    }
}
