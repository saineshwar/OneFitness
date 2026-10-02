using System;
using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class DateRangeReportRequestViewModel
    {
        [Required]
        public DateTime? FromDate { get; set; }

        [Required]
        public DateTime? ToDate { get; set; }
    }
}
