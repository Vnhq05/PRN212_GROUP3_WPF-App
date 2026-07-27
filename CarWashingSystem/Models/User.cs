using System;

namespace CarWashingSystem.Models
{
    // Model đại diện cho người dùng hệ thống
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
