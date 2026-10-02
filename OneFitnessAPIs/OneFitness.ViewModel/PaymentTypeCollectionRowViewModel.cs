namespace OneFitness.ViewModel
{
    public class PaymentTypeCollectionRowViewModel
    {
        public int? PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; } = string.Empty;
        public int TransactionCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
