using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OneFitness.Entities
{
    [Table("MenuMaster")]
    public class MenuMaster
    {
        [Key]
        public int MenuId { get; set; }

        [Required, MaxLength(100)]
        public string MenuName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Area { get; set; }

        [Required, MaxLength(100)]
        public string ControllerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ActionMethod { get; set; } = string.Empty;

        public bool Status { get; set; }

        public int? MenuCategoryId { get; set; }

        public int? RoleId { get; set; }

        public int? SortingOrder { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedOn { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
