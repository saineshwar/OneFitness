using System;

namespace OneFitness.ViewModel
{
    public class ReceiptHistoryViewModel
    {
        public long ReceiptHistoryId { get; set; }
        public long? InvoiceNo { get; set; }
        public string MemberNo { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
    }
}
