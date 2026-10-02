namespace OneFitness.ViewModel
{
    public class TaxSummaryRowViewModel
    {
        public int? TaxId { get; set; }
        public string TaxType { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public int TransactionCount { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
