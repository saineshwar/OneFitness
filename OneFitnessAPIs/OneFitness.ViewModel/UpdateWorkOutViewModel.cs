using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class UpdateWorkOutViewModel
    {
        [Required, MaxLength(100)]
        public string WorkOutName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
