using System;

namespace OneFitness.ViewModel
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public string? Gender { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool Status { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
