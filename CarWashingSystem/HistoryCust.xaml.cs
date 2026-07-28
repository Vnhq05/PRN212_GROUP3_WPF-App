using CarWashingSystem.Entities;
using CarWashingSystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace CarWashingSystem
{
    /// <summary>
    /// Interaction logic for HistoryCust.xaml
    /// </summary>
    public partial class HistoryCust : Window
    {
        private CarWashingSystemDbContext _db;

        public HistoryCust()
        {
            InitializeComponent();
            _db = new();
            LoadBookingData();
        }

        public void LoadBookingData()
        {
            // Id khach hang dang dang nhap
            string customerId = SessionManager.CurrentUser?.Id ?? "";

            // Include() de keo kem du lieu cua bang lien ket
            // => DataGrid moi hien duoc Customer.FullName, AssignedStaff.FullName,
            //    CustomerVehicle.LicensePlate. Khong co Include thi cac cot nay trong.
            var bookings = _db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.AssignedStaff)
                .Include(b => b.CustomerVehicle)
                .Where(b => b.CustomerId == customerId && !b.IsDeleted)
                .OrderByDescending(b => b.ScheduledStartTime)
                .ToList();

            // 1. Tính tổng số lần rửa xe (chỉ của khách đang đăng nhập)
            txtTotalWashes.Text = $"{bookings.Count} lần";

            // 2. Tính tổng số tiền đã chi tiêu (bỏ qua đơn đã hủy)
            var totalSpent = bookings.Where(b => b.Status != "Cancelled").Sum(b => b.TotalPrice);
            txtTotalSpent.Text = $"{totalSpent:N0} VNĐ";

            // 3. Đổ dữ liệu vào bảng DataGrid
            dgHistory.ItemsSource = bookings;
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            string? bookingId = btn?.Tag?.ToString();
            if (string.IsNullOrEmpty(bookingId)) return;

            var ratingPage = new RatingPage(bookingId);
            ratingPage.ShowDialog();
        }

        // ===== TOPNAV =====
        private void navBooking_Click(object sender, RoutedEventArgs e)
        {
            new BookingPage().Show();
            this.Close();
        }

        private void navProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfilePage().Show();
            this.Close();
        }

        private void navLogout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.SignOut();
            new LoginPage().Show();
            this.Close();
        }
    }
}
