using System;

namespace UserManagementApp.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsBlocked { get; set; }
    }
}
