using System.ComponentModel.DataAnnotations;

namespace OneFitness.ViewModel
{
    public class CreateMembershipTypeViewModel
    {
        [Required, MaxLength(100)]
        public string MembershipTypeName { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int InstallmentId { get; set; }

        [Required]
        public int WorkOutId { get; set; }

        public bool Status { get; set; }
    }
}
