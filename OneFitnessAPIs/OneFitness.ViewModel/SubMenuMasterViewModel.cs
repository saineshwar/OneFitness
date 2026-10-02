using System;

namespace OneFitness.ViewModel
{
    public class SubMenuMasterViewModel
    {
        public int SubMenuId { get; set; }
        public string SubMenuName { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string ControllerName { get; set; } = string.Empty;
        public string ActionMethod { get; set; } = string.Empty;
        public bool Status { get; set; }
        public int MenuId { get; set; }
        public int? MenuCategoryId { get; set; }
        public int? RoleId { get; set; }
        public int? SortingOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
