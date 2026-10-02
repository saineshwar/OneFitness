using System;

namespace OneFitness.ViewModel
{
    public class PaymentTypeViewModel
    {
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
