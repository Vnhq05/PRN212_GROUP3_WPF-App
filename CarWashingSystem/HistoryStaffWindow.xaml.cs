using System.Linq;
using System.Windows;
using CarWashingSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashingSystem
{
    public partial class HistoryStaffWindow : Window
    {
        public HistoryStaffWindow()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            using (var db = new CarWashingSystemDbContext())
            {
                var historyJobs = db.Bookings
                             .Include(b => b.Customer)
                             .Include(b => b.CustomerVehicle)
                             .Where(b => b.Status == "Completed")
                             .OrderByDescending(b => b.ScheduledStartTime)
                             .ToList();
                             
                dgHistoryJobs.ItemsSource = historyJobs;
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
