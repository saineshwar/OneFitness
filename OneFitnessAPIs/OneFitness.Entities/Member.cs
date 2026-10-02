using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("MemberRegistration")]
    public class Member
    {
        [Key]
        public long MemberId { get; set; }

        [Required, MaxLength(50)]
        public string MemberNo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        public DateTime? DOB { get; set; }

        public int? Age { get; set; }

        [MaxLength(10)]
        public string? MobileNo { get; set; }

        [MaxLength(100)]
        public string? EmailId { get; set; }

        public int? GenderId { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        public DateTime? JoiningDate { get; set; }

        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(15)]
        public string? EmergencyContactNo { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
