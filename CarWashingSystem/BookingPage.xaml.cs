using CarWashingSystem.Entities;
using CarWashingSystem.Services;
using System.Windows;

namespace CarWashingSystem
{
    /// <summary>
    /// Interaction logic for BookingPage.xaml
    /// </summary>
    public partial class BookingPage : Window
    {
        private CarWashingSystemDbContext _db;

        // Id cua khach hang dang dang nhap (lay tu SessionManager)

        private string CurrentCustomerId => SessionManager.CurrentUser?.Id ?? "";

        // Id 2 goi dich vu trong bang WashServices, ung voi 2 RadioButton
        private const string SrvStandardId = "SRV-STANDARD"; 
        private const string SrvPremiumId = "SRV-PREMIUM";    

        public BookingPage()
        {
            InitializeComponent();
            _db = new();

            LoadCars();
            LoadTimeSlots();

            dpDate.SelectedDate = DateTime.Today;
        }

        private void LoadCars()
        {
            var cars = _db.CustomerVehicles
                .Where(v => v.CustomerId == CurrentCustomerId && !v.IsDeleted)
                .ToList();

            cmbCars.ItemsSource = cars;
            cmbCars.DisplayMemberPath = "LicensePlate";
            cmbCars.SelectedValuePath = "Id";
            cmbCars.SelectedIndex = 0;
        }

        private void LoadTimeSlots()
        {
            var slots = new List<string>();
            for (int h = 8; h <= 17; h++)
            {
                slots.Add($"{h:00}:00");
                slots.Add($"{h:00}:30");
            }

            cmbTime.ItemsSource = slots;
            cmbTime.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCars.SelectedValue == null)
            {
                MessageBox.Show("Bạn chưa có xe nào. Vui lòng thêm xe ở trang Cá nhân.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbTime.SelectedItem is not string timeText)
            {
                MessageBox.Show("Vui lòng chọn giờ.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (rbBasic.IsChecked != true && rbPremium.IsChecked != true)
            {
                MessageBox.Show("Vui lòng chọn gói dịch vụ.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string serviceId;
            if (rbBasic.IsChecked == true)
            {
                serviceId = SrvStandardId;
            }
            else
            {
                serviceId = SrvPremiumId;
            }

            var service = _db.WashServices.FirstOrDefault(s => s.Id == serviceId);
            if (service == null)
            {
                MessageBox.Show("Không tìm thấy gói dịch vụ trong hệ thống.", "Lỗi dữ liệu",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime date = dpDate.SelectedDate.Value;
            string[] parts = timeText.Split(':');
            DateTime start = date.AddHours(int.Parse(parts[0]))
                                 .AddMinutes(int.Parse(parts[1]));

            if (start < DateTime.Now)
            {
                MessageBox.Show("Không thể đặt lịch vào thời điểm đã qua.", "Ngày giờ không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var booking = new Booking
            {
                Id = "BKG-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                CustomerId = CurrentCustomerId,
                CustomerVehicleId = (string)cmbCars.SelectedValue,
                BookingDate = DateOnly.FromDateTime(date),
                ScheduledStartTime = start,
                ScheduledEndTime = start.AddMinutes(service.DurationMinutes),
                Status = "Pending",
                TotalPrice = service.Price
            };


            booking.Services.Add(service);
            _db.Bookings.Add(booking);
            _db.SaveChanges();

            MessageBox.Show($"Đặt lịch thành công!\nMã đặt lịch: {booking.Id}", "Thành công",
                MessageBoxButton.OK, MessageBoxImage.Information);

            // Truyen ma don sang man hinh Payment
            new Payment(booking.Id).Show();
            this.Close();
        }

        // ===== TOPNAV =====
        private void navProfile_Click(object sender, RoutedEventArgs e)
        {
            new ProfilePage().Show();
            this.Close();
        }

        private void navHistory_Click(object sender, RoutedEventArgs e)
        {
            new HistoryCust().Show();
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
