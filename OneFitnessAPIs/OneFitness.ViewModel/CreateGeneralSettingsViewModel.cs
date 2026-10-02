using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateGeneralSettingsViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string SupportEmailId { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string WebsiteTitle { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? WebsiteUrl { get; set; }

        [MaxLength(15)]
        public string? TelephoneNo { get; set; }

        [MaxLength(10)]
        public string? MobileNo { get; set; }

        public bool Status { get; set; }

        [MaxLength(100)]
        public string? Logopath { get; set; }

        [MaxLength(100)]
        public string? LogoFileName { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;
    }
}
