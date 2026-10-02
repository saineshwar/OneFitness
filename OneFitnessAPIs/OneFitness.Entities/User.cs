using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required, MaxLength(256)]
        public string EmailId { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        public bool Status { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsFirstLogin { get; set; } = true;

        public DateTime? IsFirstLoginDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
