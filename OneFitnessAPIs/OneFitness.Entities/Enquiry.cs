using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("Enquiry")]
    public class Enquiry
    {
        [Key]
        public int EnquiryId { get; set; }

        public int? WorkOutId { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [MaxLength(10)]
        public string? MobileNo { get; set; }

        [MaxLength(100)]
        public string? EmailId { get; set; }

        public int? GenderId { get; set; }

        public int? ReasonId { get; set; }

        [MaxLength(100)]
        public string? EnquiryDetails { get; set; }

        public bool Status { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
