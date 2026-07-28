using CarWashingSystem.Entities;
using CarWashingSystem.Services;
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
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Window
    {
        private CarWashingSystemDbContext _db;
        public ProfilePage()
        {
            InitializeComponent();
            _db = new();
            GetProfile();
            LoadCustomerVehicles();
        }
        private void GetProfile()
        {
            if (SessionManager.IsLoggedIn)
            {
                var user = SessionManager.CurrentUser;
                txtUserName.Text = user.FullName;
                txtUserMail.Text = user.Email;
                txtUserPhone.Text = user.PhoneNumber;
            }
        }

        private void LoadCustomerVehicles()
        {
            string userId = SessionManager.CurrentUser.Id;
            var vehicles = _db.CustomerVehicles.
                Where(v => v.CustomerId == userId).
                Where(v => v.IsDeleted == false).ToList();
            dgVehicle.ItemsSource = vehicles;
        }

        private void dgVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgVehicle.SelectedItem is CustomerVehicle v)
            {
                txtLicensePlate.Text = v.LicensePlate;
                txtModel.Text = v.VehicleModel;
                cbxColor.SelectedValue = v.Color;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu từ giao diện
            string licensePlate = txtLicensePlate.Text.Trim();
            string model = txtModel.Text.Trim();
            string color = cbxColor.SelectedValue != null ? cbxColor.SelectedValue.ToString() : "";

            string cleanLicensePlate = licensePlate.Replace(" ", "").Trim().ToUpper();

            // 2. CHECK RỖNG (Không được bỏ trống các trường thông tin)
            if (string.IsNullOrEmpty(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLicensePlate.Focus();
                return;
            }

            if (string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Vui lòng nhập thương hiệu / dòng xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtModel.Focus();
                return;
            }

            if (string.IsNullOrEmpty(color))
            {
                MessageBox.Show("Vui lòng chọn màu xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbxColor.Focus();
                return;
            }

            // 3. CHECK TRÙNG BIỂN SỐ XE TRONG DATABASE
            // So sánh không phân biệt hoa thường và bỏ qua khoảng trắng thừa
            bool isExist = _db.CustomerVehicles
                   .Any(v => !v.IsDeleted &&
                             v.LicensePlate.ToUpper() == cleanLicensePlate);

            if (isExist)
            {
                MessageBox.Show($"Biển số xe '{licensePlate}' đã tồn tại trong hệ thống!", "Lỗi trùng lặp", MessageBoxButton.OK, MessageBoxImage.Error);
                txtLicensePlate.Focus();
                return;
            }

            // 4. LẤY THÔNG TIN USER HIỆN TẠI
            var currentUser = SessionManager.CurrentUser!;
            string customerId = currentUser.Id; // Ví dụ: "USR-CUST-01"

            // 5. TẠO ID MỚI CHO XE (Định dạng chuẩn: VEH-CUST01-01)
            // Đếm tổng số xe (bao gồm cả xe đã xóa nếu muốn ID tăng tiến liên tục, tránh trùng khóa chính)
            int count = _db.CustomerVehicles.Count(v => v.CustomerId == customerId) + 1;

            // Chuẩn hóa customerId: "USR-CUST-01" -> "VEH-CUST01"
            string baseId = customerId.Replace("USR-CUST-", "VEH-CUST");

            // Tạo ID xe mới: "VEH-CUST01" + "-" + "01" -> "VEH-CUST01-01"
            string newVehicleId = $"{baseId}-{count:D2}";

            // 6. TẠO OBJECT VÀ LƯU VÀO DATABASE
            try
            {
                var newVehicle = new CustomerVehicle
                {
                    Id = newVehicleId,
                    CustomerId = customerId,
                    LicensePlate = cleanLicensePlate,
                    VehicleModel = model,
                    Color = color,
                    CreatedAt = DateTime.Now
                };

                _db.CustomerVehicles.Add(newVehicle);
                _db.SaveChanges(); // Lưu xuống SQL

                // 7. CẬP NHẬT TRẠNG THÁI GIAO DIỆN
                LoadCustomerVehicles(); // Reload lại DataGrid
                ClearForm();            // Xóa trắng các input
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Hàm hỗ trợ xóa trắng form sau khi thêm thành công
        private void ClearForm()
        {
            txtLicensePlate.Text = "";
            txtModel.Text = "";
            cbxColor.SelectedIndex = -1;
            dgVehicle.SelectedItem = null;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // 1. KIỂM TRA XEM NGƯỜI DÙNG ĐÃ CHỌN XE NÀO TRONG DATAGRID CHƯA
            if (dgVehicle.SelectedItem is not CustomerVehicle selectedVehicle)
            {
                MessageBox.Show("Vui lòng chọn chiếc xe bạn muốn cập nhật từ danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy dữ liệu từ giao diện
            string licensePlate = txtLicensePlate.Text.Trim();
            string model = txtModel.Text.Trim();
            string color = cbxColor.SelectedValue != null ? cbxColor.SelectedValue.ToString() : "";

            string cleanLicensePlate = licensePlate.Replace(" ", "").Trim().ToUpper();

            // 2. CHECK RỖNG (Không được bỏ trống)
            if (string.IsNullOrEmpty(licensePlate))
            {
                MessageBox.Show("Vui lòng nhập biển số xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLicensePlate.Focus();
                return;
            }

            if (string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Vui lòng nhập thương hiệu / dòng xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtModel.Focus();
                return;
            }

            if (string.IsNullOrEmpty(color))
            {
                MessageBox.Show("Vui lòng chọn màu xe!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbxColor.Focus();
                return;
            }

            // 3. CHECK TRÙNG BIỂN SỐ XE (LOẠI TRỪ CHÍNH XE ĐANG SỬA)
            // Rất quan trọng: v.Id != selectedVehicle.Id
            bool isExist = _db.CustomerVehicles
                   .Any(v => !v.IsDeleted &&
                             v.Id != selectedVehicle.Id &&
                             v.LicensePlate.ToUpper()== cleanLicensePlate);

            if (isExist)
            {
                MessageBox.Show($"Biển số xe '{licensePlate}' đã được sử dụng bởi một xe khác trong hệ thống!", "Lỗi trùng lặp", MessageBoxButton.OK, MessageBoxImage.Error);
                txtLicensePlate.Focus();
                return;
            }

            // 4. BẮT LỖI TRY-CATCH KHI LƯU CSDL
            try
            {
                // Lấy đối tượng xe từ DB lên để theo dõi (Tracked) và cập nhật
                var vehicleInDb = _db.CustomerVehicles.FirstOrDefault(v => v.Id == selectedVehicle.Id);

                if (vehicleInDb != null)
                {
                    // Cập nhật các trường thông tin mới
                    vehicleInDb.LicensePlate = cleanLicensePlate;
                    vehicleInDb.VehicleModel = model;
                    vehicleInDb.Color = color;

                    // Lưu thay đổi xuống SQL Server
                    _db.SaveChanges();

                    MessageBox.Show("Cập nhật thông tin xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Cập nhật lại giao diện
                    LoadCustomerVehicles(); // Reload DataGrid
                    ClearForm();            // Xóa trắng ô nhập liệu
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu chiếc xe này trong Database!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string errorDetails = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {errorDetails}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // 1. KIỂM TRA XEM NGƯỜI DÙNG ĐÃ CHỌN XE NÀO TRONG DATAGRID CHƯA
            if (dgVehicle.SelectedItem is not CustomerVehicle selectedVehicle)
            {
                MessageBox.Show("Vui lòng chọn chiếc xe bạn muốn xóa từ danh sách!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. HIỂN THỊ CẢNH BÁO XÁC NHẬN (CONFIRMATION)
            var confirmResult = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa xe biển số '{selectedVehicle.LicensePlate}' ({selectedVehicle.VehicleModel}) không?\n\nHành động này không thể hoàn tác!",
                "Xác nhận xóa xe",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            // Nếu người dùng chọn "No" thì dừng lại, không xóa
            if (confirmResult == MessageBoxResult.No) return;

            // 3. BẮT LỖI TRY-CATCH KHI XÓA DƯỚI CSDL
            try
            {
                var vehicleInDb = _db.CustomerVehicles.FirstOrDefault(v => v.Id == selectedVehicle.Id);

                if (vehicleInDb != null)
                {
                    // KHÔNG DÙNG: _db.CustomerVehicles.Remove(vehicleInDb);

                    // ĐỔI THÀNH: Cập nhật cờ IsDeleted = true (hoặc 1 tùy kiểu bool/int trong Entity)
                    vehicleInDb.IsDeleted = true;

                    _db.SaveChanges(); // Lưu trạng thái xuống SQL

                    MessageBox.Show("Đã xóa xe thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadCustomerVehicles();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu chiếc xe này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadCustomerVehicles();
                }
            }
            catch (Exception ex)
            {
                string errorDetails = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Lỗi khi xóa dữ liệu: {errorDetails}", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }
    }
}
