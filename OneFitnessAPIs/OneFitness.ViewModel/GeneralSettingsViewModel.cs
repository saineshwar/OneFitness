using System;

namespace OneFitness.ViewModel
{
    public class GeneralSettingsViewModel
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SupportEmailId { get; set; } = string.Empty;
        public string WebsiteTitle { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
        public string? TelephoneNo { get; set; }
        public string? MobileNo { get; set; }
        public bool Status { get; set; }
        public string? Logopath { get; set; }
        public string? LogoFileName { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
