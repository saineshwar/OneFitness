using System;

namespace OneFitness.ViewModel
{
    public class EnquiryViewModel
    {
        public int EnquiryId { get; set; }
        public int? WorkOutId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public int? GenderId { get; set; }
        public int? ReasonId { get; set; }
        public string? EnquiryDetails { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
