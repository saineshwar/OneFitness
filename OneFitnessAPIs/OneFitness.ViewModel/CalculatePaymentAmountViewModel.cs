namespace OneFitness.ViewModel
{
    public class CalculatePaymentAmountViewModel
    {
        public int MembershipTypeId { get; set; }
        public int TaxId { get; set; }
    }

    public class PaymentAmountCalculationViewModel
    {
        public decimal Amount { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal TaxPercentageAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
