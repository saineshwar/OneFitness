using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class UpdateTaxMasterViewModel
    {
        [Required, MaxLength(10)]
        public string TaxType { get; set; } = string.Empty;

        [Required]
        public decimal TaxRate { get; set; }

        public bool Status { get; set; }

        [MaxLength(50)]
        public string? IdentificationNo { get; set; }
    }
}
