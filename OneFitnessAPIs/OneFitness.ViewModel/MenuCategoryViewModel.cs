using System;

namespace OneFitness.ViewModel
{
    public class MenuCategoryViewModel
    {
        public int MenuCategoryId { get; set; }
        public string MenuCategoryName { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public int? SortingOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
