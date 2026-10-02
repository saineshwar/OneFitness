using System;

namespace OneFitness.ViewModel
{
    public class LoginResultViewModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
    }
}
