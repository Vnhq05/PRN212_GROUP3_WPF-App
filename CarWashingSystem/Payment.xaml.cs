using CarWashingSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CarWashingSystem
{
    /// <summary>
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment : Window
    {
        private CarWashingSystemDbContext _db;

   
        private string _bookingId;
        private Booking? _booking;

        public Payment(string bookingId)
        {
            InitializeComponent();
            _db = new();
            _bookingId = bookingId;

            LoadBooking();
        }

        private void LoadBooking()
        {
            _booking = _db.Bookings
                .Include(b => b.CustomerVehicle)
                .Include(b => b.Services)
                .FirstOrDefault(b => b.Id == _bookingId && !b.IsDeleted);

            if (_booking == null)
            {
                MessageBox.Show("Không tìm thấy đơn đặt lịch.", "Lỗi dữ liệu",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                btnPay.IsEnabled = false;
                return;
            }

            if (_booking.CustomerVehicle != null)
            {
                txtVehicle.Text = $"{_booking.CustomerVehicle.VehicleModel} - {_booking.CustomerVehicle.LicensePlate}";
            }
            else
            {
                txtVehicle.Text = "(chưa chọn xe)";
            }

            txtDateTime.Text = _booking.ScheduledStartTime.ToString("dd/MM/yyyy - HH:mm");
            txtService.Text = string.Join(", ", _booking.Services.Select(s => s.ServiceName));
            txtTotal.Text = $"{_booking.TotalPrice:N0}đ";

            var daThanhToan = _db.Invoices
                .Any(i => i.BookingId == _bookingId
                       && i.PaymentStatus == "Paid"
                       && !i.IsDeleted);

            if (daThanhToan)
            {
                MessageBox.Show("Đơn này đã được thanh toán.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                btnPay.IsEnabled = false;
            }
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {
            if (_booking == null)
            {
                return;
            }
            string paymentMethod;
            if (rdoCash.IsChecked == true)
            {
                paymentMethod = "Cash";
            }
            else if (rdoMomo.IsChecked == true)
            {
                paymentMethod = "Momo";
            }
            else if (rdoCreditCard.IsChecked == true)
            {
                paymentMethod = "Card";
            }
            else
            {
                MessageBox.Show("Vui lòng chọn phương thức thanh toán.", "Thiếu thông tin",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var invoice = new Invoice
            {
                Id = "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                BookingId = _booking.Id,
                OriginalAmount = _booking.TotalPrice,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Paid",
                PaymentTime = DateTime.Now
            };

            _db.Invoices.Add(invoice);
            _booking.Status = "Pending";
            _booking.UpdatedAt = DateTime.Now;

            _db.SaveChanges();

            MessageBox.Show(
                $"Thanh toán thành công!\n\n" +
                $"Mã hóa đơn: {invoice.Id}\n" +
                $"Phương thức: {paymentMethod}\n" +
                $"Số tiền: {invoice.FinalAmount:N0}đ",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            btnPay.IsEnabled = false;
            var HistoryCust = new HistoryCust();
            HistoryCust.Show();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Bạn có chắc muốn hủy đơn đặt lịch này?", "Xác nhận",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
          
                if (_booking != null)
                {
                    _booking.Status = "Cancelled";
                    _booking.UpdatedAt = DateTime.Now;
                    _db.SaveChanges();

                    MessageBox.Show($"Đã hủy đơn {_booking.Id}.", "Đã hủy",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                var BookingPage = new BookingPage();
                BookingPage.Show();
                this.Close();
            }
        }

    }
}
