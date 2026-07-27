using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarWashingSystem.ViewModels;

namespace CarWashingSystem
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        // Khởi tạo và set DataContext cho LoginPage
        public LoginPage()
        {
            InitializeComponent();
            // Set ViewModel cho trang đăng nhập
            this.DataContext = new LoginViewModel();
        }

        // Bắt sự kiện PasswordChanged trên PasswordBox để cập nhật Password trong ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password; // Gán mật khẩu trực tiếp (không hash) theo yêu cầu
            }
        }
    }
}
