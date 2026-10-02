using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class UpdateMenuCategoryViewModel
    {
        [Required, MaxLength(100)]
        public string MenuCategoryName { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public bool Status { get; set; }

        public int? SortingOrder { get; set; }
    }
}
