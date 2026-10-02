using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateInstallmentViewModel
    {
        [Required, MaxLength(100)]
        public string InstallmentName { get; set; } = string.Empty;

        public bool Status { get; set; }

        public int? InstallmentMonths { get; set; }
    }
}
