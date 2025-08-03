using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}