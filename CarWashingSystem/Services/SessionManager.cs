using System;
using CarWashingSystem.Models;

namespace CarWashingSystem.Services
{
    // Quản lý session đơn giản: lưu user hiện tại và token
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
        public static string CurrentToken { get; set; }
    }
}
