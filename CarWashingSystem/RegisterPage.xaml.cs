using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarWashingSystem
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            txtError.Text = string.Empty;

            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = pwdPassword.Password;
            string confirm = pwdConfirm.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                txtError.Text = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }

            if (!email.Contains("@"))
            {
                txtError.Text = "Email không hợp lệ.";
                return;
            }

            if (password != confirm)
            {
                txtError.Text = "Mật khẩu xác nhận không khớp.";
                return;
            }

            using var db = new Entities.CarWashingSystemDbContext();

            bool exists = db.Users.Any(u => !u.IsDeleted && (u.Email == email || u.PhoneNumber == phone));
            if (exists)
            {
                txtError.Text = "Email hoặc số điện thoại đã được sử dụng.";
                return;
            }

            var user = new Entities.User
            {
                Id = "USR-CUST-" + Guid.NewGuid().ToString("N").Substring(0, 8),
                FullName = username,
                Email = email,
                PhoneNumber = phone,
                Password = password,
                RoleId = "ROL-CUST",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            MessageBox.Show("Đăng ký thành công. Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            var login = new LoginPage();
            login.Show();
            this.Close();
        }

        private void BtnSwitchToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginPage();
            login.Show();
            this.Close();
        }
    }
}
