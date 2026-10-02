using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class UpdateEnquiryViewModel
    {
        public int? WorkOutId { get; set; }

        [Required, MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid First Name")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid Last Name")]
        public string? LastName { get; set; }

        [MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid Middle Name")]
        public string? MiddleName { get; set; }

        [MaxLength(10), RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Enter valid Mobile No.")]
        public string? MobileNo { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? EmailId { get; set; }

        [Required]
        public int? GenderId { get; set; }

        public int? ReasonId { get; set; }

        [MaxLength(100)]
        public string? EnquiryDetails { get; set; }

        public bool Status { get; set; }
    }
}
