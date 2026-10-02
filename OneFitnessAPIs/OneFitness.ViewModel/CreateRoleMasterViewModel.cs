using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateRoleMasterViewModel
    {
        [Required, MaxLength(100)]
        public string RoleName { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
