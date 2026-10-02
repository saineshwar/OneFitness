using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Required, Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
