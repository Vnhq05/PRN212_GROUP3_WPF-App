using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarWashingSystem.Entities;
using CarWashingSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace CarWashingSystem
{
    public partial class StaffWindow : Window
    {
        public StaffWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new CarWashingSystemDbContext())
            {
                // Dùng Entity Framework Core để truy vấn DB. Include() tương đương với JOIN trong SQL để lấy thông tin Customer và Vehicle.
                // Lọc ra các công việc đang ở trạng thái chưa hoàn thành (Pending, Confirmed, InProgress)
                var jobs = db.Bookings
                             .Include(b => b.Customer)
                             .Include(b => b.CustomerVehicle)
                             .Where(b => b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "InProgress")
                             .OrderBy(b => b.ScheduledStartTime)
                             .ToList();
                             
                // Đổ dữ liệu vào DataGrid (Data Binding)
                dgCurrentJobs.ItemsSource = jobs;
            }
        }

        private void btnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            
            // Lấy ID của Booking từ thuộc tính Tag đã được gán (Binding) ở file XAML
            string bookingId = btn.Tag?.ToString();

            if (!string.IsNullOrEmpty(bookingId))
            {
                using (var db = new CarWashingSystemDbContext())
                {
                    var booking = db.Bookings.FirstOrDefault(b => b.Id == bookingId);
                    if (booking != null)
                    {
                        // Chỉ được đổi trạng thái khi thời gian hiện tại đã đến gần (trước 30 phút) hoặc đã quá giờ đặt
                        if (System.DateTime.Now < booking.ScheduledStartTime.AddMinutes(-30))
                        {
                            MessageBox.Show($"Chưa đến lúc! Bạn chỉ có thể đổi trạng thái bắt đầu từ {booking.ScheduledStartTime.AddMinutes(-30):HH:mm dd/MM/yyyy} (trước giờ hẹn 30 phút).", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Ghi nhận nhân viên thực hiện thao tác
                        if (SessionManager.CurrentUser == null)
                        {
                            MessageBox.Show("Lỗi: Không tìm thấy thông tin nhân viên đang đăng nhập!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        booking.AssignedStaffId = SessionManager.CurrentUser.Id;

                        // Logic chuyển đổi trạng thái công việc tịnh tiến
                        if (booking.Status == "Pending") 
                        {
                            booking.Status = "Confirmed";
                        }
                        else if (booking.Status == "Confirmed") 
                        {
                            booking.Status = "InProgress";
                            booking.CheckInTime = System.DateTime.Now; // Ghi nhận giờ bắt đầu rửa
                        }
                        else if (booking.Status == "InProgress") 
                        {
                            booking.Status = "Completed";
                            booking.CheckoutTime = System.DateTime.Now; // Ghi nhận giờ hoàn thành
                        }

                        // Lưu thay đổi xuống Database
                        db.SaveChanges();
                        // Tải lại lưới dữ liệu để cập nhật UI
                        LoadData(); 
                    }
                }
            }
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            HistoryStaffWindow historyWindow = new HistoryStaffWindow();
            historyWindow.Show();
            this.Close();
        }

        private void btnFeedback_Click(object sender, RoutedEventArgs e)
        {
            FeedbackWindow fw = new FeedbackWindow();
            fw.Show();
            this.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Clear session and return to login page
            SessionManager.SignOut();

            var login = new LoginPage();
            login.Show();
            this.Close();
        }
    }
}
