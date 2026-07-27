using CarWashingSystem.Entities;
using Microsoft.EntityFrameworkCore;
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
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private CarWashingSystemDbContext _db;
        public Dashboard()
        {
            InitializeComponent();
            _db = new();
            LoadBookingData();
        }

        public void LoadBookingData()
        {
            var bookings = _db.Bookings.ToList();

            // 1. Tính tổng số lần rửa xe (Đếm tổng số đơn trong DB)
            txtTotalWashes.Text = $"{bookings.Count} lần";

            // 2. Tính tổng số tiền đã chi tiêu (Cộng tổng cột TotalPrice)
            var totalSpent = bookings.Sum(b => b.TotalPrice);
            txtTotalSpent.Text = $"{totalSpent:N0} VNĐ";

            // 3. Đổ dữ liệu vào bảng DataGrid
            dgHistory.ItemsSource = bookings;
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as System.Windows.Controls.Button;
            string bookingId = btn?.Tag?.ToString() ?? "BKG-001";
            var ratingPage = new RatingPage(bookingId);
            ratingPage.ShowDialog();
        }
    }
}
