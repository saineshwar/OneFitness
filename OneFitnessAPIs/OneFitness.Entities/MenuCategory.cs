using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("MenuCategory")]
    public class MenuCategory
    {
        [Key]
        public int MenuCategoryId { get; set; }

        [Required, MaxLength(100)]
        public string MenuCategoryName { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public bool Status { get; set; }

        public int? SortingOrder { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
