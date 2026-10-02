using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateUserViewModel
    {
        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string EmailId { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [Required]
        public int? RoleId { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
