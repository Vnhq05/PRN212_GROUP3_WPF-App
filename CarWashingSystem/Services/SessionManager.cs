using CarWashingSystem.Entities;

namespace CarWashingSystem.Services
{
    public static class SessionManager
    {
        // Lưu thông tin user đang đăng nhập, dùng chung cho cả app
        public static User? CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void SignIn(User user)
        {
            CurrentUser = user;
        }

        public static void SignOut()
        {
            CurrentUser = null;
        }
    }
}