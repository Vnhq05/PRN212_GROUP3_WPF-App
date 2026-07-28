using System.Linq;
using System.Windows;
using CarWashingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashingSystem
{
    public partial class FeedbackWindow : Window
    {
        public FeedbackWindow()
        {
            InitializeComponent();
            LoadFeedback();
        }

        private void LoadFeedback()
        {
            using (var db = new CarWashingSystemDbContext())
            {
                // Truy vấn bảng ServiceReviews (Đánh giá) và JOIN với bảng Customer để lấy tên khách hàng
                // Sắp xếp mới nhất lên đầu
                var feedbacks = db.ServiceReviews
                                  .Include(r => r.Customer)
                                  .OrderByDescending(r => r.CreatedAt)
                                  .ToList();
                dgFeedback.ItemsSource = feedbacks;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            StaffWindow staffWindow = new StaffWindow();
            staffWindow.Show();
            this.Close();
        }
    }
}
