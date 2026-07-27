using System;

namespace CarWashingSystem.Services
{
    // Dịch vụ tạo token đơn giản bằng Guid
    public static class TokenService
    {
        public static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
