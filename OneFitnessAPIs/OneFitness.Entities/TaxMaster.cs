using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("TaxMaster")]
    public class TaxMaster
    {
        [Key]
        public int TaxId { get; set; }

        [Required, MaxLength(10)]
        public string TaxType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxRate { get; set; }

        public bool Status { get; set; }

        [MaxLength(50)]
        public string? IdentificationNo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
