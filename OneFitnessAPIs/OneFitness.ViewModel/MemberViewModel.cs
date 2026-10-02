using System;

namespace OneFitness.ViewModel
{
    public class MemberViewModel
    {
        public long MemberId { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime? DOB { get; set; }
        public int? Age { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public int? GenderId { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime? JoiningDate { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNo { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
