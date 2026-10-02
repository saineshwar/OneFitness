using System;

namespace OneFitness.ViewModel
{
    public class InstallmentViewModel
    {
        public int InstallmentId { get; set; }
        public string InstallmentName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int? InstallmentMonths { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
