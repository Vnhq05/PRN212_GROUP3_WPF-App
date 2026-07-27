using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CarWashingSystem.Helpers;
using CarWashingSystem.Models;
using CarWashingSystem.Repositories;
using CarWashingSystem.Services;

namespace CarWashingSystem.ViewModels
{
    // ViewModel cho trang đăng nhập
    public class LoginViewModel : BaseViewModel
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        // Password sẽ được gán từ code-behind (PasswordBox không hỗ trợ binding)
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        // Lệnh đăng nhập
        public ICommand LoginCommand { get; }

        // Lệnh chuyển sang đăng ký (stub để tránh lỗi binding nếu có)
        public ICommand SwitchToRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => ExecuteLogin(), o => CanExecuteLogin());
            SwitchToRegisterCommand = new RelayCommand(o => ExecuteSwitchToRegister());
        }

        // Kiểm tra có thể thực hiện đăng nhập hay không
        private bool CanExecuteLogin()
        {
            // Cho phép khi username và password không rỗng (bỏ khoảng trắng 2 đầu)
            return !string.IsNullOrWhiteSpace(Username?.Trim()) && !string.IsNullOrWhiteSpace(Password?.Trim());
        }

        // Thực hiện đăng nhập
        private void ExecuteLogin()
        {
            // Lấy giá trị thực nhận và chuẩn hóa trước khi so sánh
            var usernameDebug = Username?.Trim();
            var passwordDebug = Password ?? string.Empty; // giữ nguyên password nhập để so sánh chính xác

            // Tìm user trong fake repo (so sánh chuỗi mật khẩu trực tiếp)
            var user = FakeUserRepository.GetByUsernameAndPassword(usernameDebug, passwordDebug);
            if (user == null)
            {
                // Hiển thị lỗi khi sai tên đăng nhập hoặc mật khẩu
                MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Nếu đúng, sinh token và lưu vào SessionManager
            var token = TokenService.GenerateToken();
            SessionManager.CurrentUser = user;
            SessionManager.CurrentToken = token;

            // Điều hướng theo role
            NavigateByRole(user.Role);
        }

        // Hàm chuyển hướng theo role
        public void NavigateByRole(string role)
        {
            // Mở cửa sổ tương ứng
            if (string.Equals(role, "Staff", StringComparison.OrdinalIgnoreCase))
            {
                var staffWin = new CarWashingSystem.StaffWindow();
                staffWin.Show();
            }
            else if (string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
            {
                var profileWin = new CarWashingSystem.ProfilePage();
                profileWin.Show();
            }
            else
            {
                // Nếu role lạ thì thông báo
                MessageBox.Show("Role không hợp lệ.", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Đóng LoginPage hiện tại
            var loginWindow = System.Windows.Application.Current.Windows
                              .OfType<CarWashingSystem.LoginPage>()
                              .FirstOrDefault();
            loginWindow?.Close();
        }

        // Stub cho chuyển sang màn hình đăng ký (chỉ thông báo)
        private void ExecuteSwitchToRegister()
        {
            MessageBox.Show("Chức năng đăng ký chưa được triển khai trong demo.", "Thông tin", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
