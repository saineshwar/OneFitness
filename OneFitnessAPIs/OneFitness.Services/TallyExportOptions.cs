namespace OneFitness.Services
{
    public class TallyExportOptions
    {
        public string CompanyName { get; set; } = "OneGYM";
        public string CashLedger { get; set; } = "Cash";
        public string IncomeLedger { get; set; } = "Membership Income";
        public string RefundLedger { get; set; } = "Membership Refunds";
        public string GstLedger { get; set; } = "Output GST";
        public string ReceiptVoucherType { get; set; } = "Receipt";
        public string PaymentVoucherType { get; set; } = "Payment";
    }
}
