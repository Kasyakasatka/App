using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UserManagementApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsBlocked { get; set; } = false;
        public DateTime LastLoginTime { get; set; }
        public string Name { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}