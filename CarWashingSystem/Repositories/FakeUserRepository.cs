using System;
using System.Collections.Generic;
using System.Linq;
using CarWashingSystem.Models;

namespace CarWashingSystem.Repositories
{
    // Kho chứa danh sách user mẫu (hard-code) để test
    public static class FakeUserRepository
    {
        // Danh sách user mẫu
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Username = "staff1", Password = "staffpass", Role = "Staff" },
            new User { Id = 2, Username = "user1", Password = "userpass", Role = "User" },
            // Có thể thêm nhiều user mẫu khác nếu cần
        };

        // Lấy user theo username và password (so sánh chuỗi trực tiếp)
        public static User GetByUsernameAndPassword(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                                            && u.Password == password);
        }

        // Trả về tất cả user (nếu cần dùng ở nơi khác)
        public static IEnumerable<User> GetAll() => _users;
    }
}
