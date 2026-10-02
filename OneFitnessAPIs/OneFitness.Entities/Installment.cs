using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("Installments")]
    public class Installment
    {
        [Key]
        public int InstallmentId { get; set; }

        [Required, MaxLength(100)]
        public string InstallmentName { get; set; } = string.Empty;

        public bool Status { get; set; }

        public int? InstallmentMonths { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
