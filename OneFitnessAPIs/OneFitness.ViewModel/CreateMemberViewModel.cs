using System;
using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateMemberViewModel
    {
        [Required, MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid First Name")]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid Last Name")]
        public string? LastName { get; set; }

        [MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid Middle Name")]
        public string? MiddleName { get; set; }

        [Required]
        public DateTime? DOB { get; set; }

        [Required]
        public int? Age { get; set; }

        [MaxLength(10)]
        public string? MobileNo { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? EmailId { get; set; }

        [Required]
        public int? GenderId { get; set; }

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime? JoiningDate { get; set; }

        [MaxLength(100), RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "Enter valid Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [MaxLength(15)]
        public string? EmergencyContactNo { get; set; }

        public bool Status { get; set; }
    }
}
