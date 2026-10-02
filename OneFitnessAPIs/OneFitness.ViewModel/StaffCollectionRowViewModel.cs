namespace OneFitness.ViewModel
{
    public class StaffCollectionRowViewModel
    {
        public int? UserId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public int ReceiptCount { get; set; }
        public decimal ReceiptTotal { get; set; }
        public int RefundCount { get; set; }
        public decimal RefundTotal { get; set; }
    }
}
