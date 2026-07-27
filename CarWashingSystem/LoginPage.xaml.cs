using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using CarWashingSystem.Entities;
using CarWashingSystem.Services;

namespace CarWashingSystem
{
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = "";

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                txtError.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return;
            }

            using var db = new CarWashingSystemDbContext();

            User? user = db.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => !u.IsDeleted &&
                    (u.Email == username || u.PhoneNumber == username));

            if (user == null)
            {
                txtError.Text = "Tài khoản không tồn tại.";
                return;
            }

            if (!user.IsActive)
            {
                txtError.Text = "Tài khoản đã bị khóa.";
                return;
            }

            if (user.Password != password)
            {
                txtError.Text = "Mật khẩu không đúng.";
                return;
            }

            // Đăng nhập thành công -> lưu vào session
            SessionManager.SignIn(user);

            // Kiểm tra role để điều hướng đúng màn hình
            bool isStaff = user.Role != null &&
                           user.Role.RoleName.Equals("Staff", System.StringComparison.OrdinalIgnoreCase);

            if (isStaff)
            {
                var staffWindow = new StaffWindow();
                staffWindow.Show();
            }
            else
            {
                var bookingPage = new BookingPage();
                bookingPage.Show();
            }

            this.Close();
        }

        private void BtnSwitchToRegister_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}