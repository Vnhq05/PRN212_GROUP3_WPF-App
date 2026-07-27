using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CarWashingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashingSystem
{
    public partial class StaffWindow : Window
    {
        private string currentStaffId = "USR-STAFF-01"; // Hardcoded for demo if needed, but not filtering by staff yet based on original logic

        public StaffWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new CarWashingSystemDbContext())
            {
                var jobs = db.Bookings
                             .Include(b => b.Customer)
                             .Include(b => b.CustomerVehicle)
                             .Where(b => b.Status == "Pending" || b.Status == "Confirmed" || b.Status == "InProgress")
                             .OrderBy(b => b.ScheduledStartTime)
                             .ToList();
                             
                dgCurrentJobs.ItemsSource = jobs;
            }
        }

        private void btnChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            string bookingId = btn.Tag?.ToString();

            if (!string.IsNullOrEmpty(bookingId))
            {
                using (var db = new CarWashingSystemDbContext())
                {
                    var booking = db.Bookings.FirstOrDefault(b => b.Id == bookingId);
                    if (booking != null)
                    {
                        if (booking.Status == "Pending") booking.Status = "Confirmed";
                        else if (booking.Status == "Confirmed") booking.Status = "InProgress";
                        else if (booking.Status == "InProgress") booking.Status = "Completed";

                        db.SaveChanges();
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
    }
}
