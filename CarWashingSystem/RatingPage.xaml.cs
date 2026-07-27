using CarWashingSystem.Entities;
using System;
using System.Linq;
using System.Windows;

namespace CarWashingSystem
{
    public partial class RatingPage : Window
    {
        private CarWashingSystemDbContext _db = new CarWashingSystemDbContext();
        private string _bookingId;

        public RatingPage(string bookingId = "BKG-001")
        {
            InitializeComponent();
            _bookingId = bookingId;
            LoadInfo();
        }

        private void LoadInfo()
        {
            var booking = _db.Bookings.FirstOrDefault(b => b.Id == _bookingId);
            if (booking != null)
            {
                txtBookingInfo.Text = $"Mã dịch vụ: {booking.Id} | Giá: {booking.TotalPrice:N0} VNĐ";
            }
            else
            {
                txtBookingInfo.Text = $"Mã dịch vụ: {_bookingId} | Đánh giá dịch vụ rửa xe";
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            int rating = 5;
            if (rb1Star.IsChecked == true) rating = 1;
            else if (rb2Star.IsChecked == true) rating = 2;
            else if (rb3Star.IsChecked == true) rating = 3;
            else if (rb4Star.IsChecked == true) rating = 4;

            var review = new ServiceReview
            {
                Id = "REV-" + DateTime.Now.Ticks.ToString().Substring(10),
                BookingId = _bookingId,
                CustomerId = "CUST-001",
                BranchId = "BR-001",
                OverallRating = rating,
                CleanlinessRating = rating,
                SpeedRating = rating,
                StaffRating = rating,
                Comment = txtComment.Text,
                CreatedAt = DateTime.Now
            };

            _db.ServiceReviews.Add(review);
            _db.SaveChanges();

            MessageBox.Show("Cảm ơn bạn đã gửi đánh giá!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
