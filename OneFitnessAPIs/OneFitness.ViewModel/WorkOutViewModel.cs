using System;

namespace OneFitness.ViewModel
{
    public class WorkOutViewModel
    {
        public int WorkOutId { get; set; }
        public string WorkOutName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
