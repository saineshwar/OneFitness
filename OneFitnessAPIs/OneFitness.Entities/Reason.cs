using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("Reasons")]
    public class Reason
    {
        [Key]
        public int ReasonId { get; set; }

        [Required, MaxLength(50)]
        public string ReasonName { get; set; } = string.Empty;
    }
}
