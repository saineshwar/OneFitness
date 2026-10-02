using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("GeneralSettings")]
    public class GeneralSettings
    {
        [Key]
        public int CompanyId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
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

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
