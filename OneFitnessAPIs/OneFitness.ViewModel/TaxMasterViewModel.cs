using System;

namespace OneFitness.ViewModel
{
    public class TaxMasterViewModel
    {
        public int TaxId { get; set; }
        public string TaxType { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public bool Status { get; set; }
        public string? IdentificationNo { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
