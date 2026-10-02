using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateMenuMasterViewModel
    {
        [Required, MaxLength(100)]
        public string MenuName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Area { get; set; }

        [Required, MaxLength(100)]
        public string ControllerName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string ActionMethod { get; set; } = string.Empty;

        public bool Status { get; set; }

        [Required]
        public int MenuCategoryId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public int? SortingOrder { get; set; }
    }
}
