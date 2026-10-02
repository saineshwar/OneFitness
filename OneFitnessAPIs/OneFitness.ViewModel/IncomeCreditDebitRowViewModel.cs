using System;

namespace OneFitness.ViewModel
{
    public class IncomeCreditDebitRowViewModel
    {
        public DateTime Date { get; set; }
        public string VoucherType { get; set; } = string.Empty;
        public string Particulars { get; set; } = string.Empty;
        public string MemberNo { get; set; } = string.Empty;
        public long? InvoiceNo { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }
    }
}
