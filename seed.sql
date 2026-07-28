USE CarWashingSystemDB;
GO

-- Seed Roles
IF NOT EXISTS(SELECT 1 FROM Roles WHERE Id = 'ROLE-STAFF')
    INSERT INTO Roles (Id, RoleName, Description) VALUES ('ROLE-STAFF', 'Staff', 'Nhân viên rửa xe');
IF NOT EXISTS(SELECT 1 FROM Roles WHERE Id = 'ROLE-CUST')
    INSERT INTO Roles (Id, RoleName, Description) VALUES ('ROLE-CUST', 'Customer', 'Khách hàng');

-- Seed Users
IF NOT EXISTS(SELECT 1 FROM Users WHERE Id = 'USR-STAFF-01')
    INSERT INTO Users (Id, RoleId, FullName, Email, Password, PhoneNumber, IsActive) 
    VALUES ('USR-STAFF-01', 'ROLE-STAFF', N'Trần Văn Nhân Viên', 'staff1@gmail.com', '123456', '0912345678', 1);

IF NOT EXISTS(SELECT 1 FROM Users WHERE Id = 'USR-CUST-01')
    INSERT INTO Users (Id, RoleId, FullName, Email, Password, PhoneNumber, IsActive) 
    VALUES ('USR-CUST-01', 'ROLE-CUST', N'Nguyễn Khách Hàng', 'cust1@gmail.com', '123456', '0987654321', 1);

IF NOT EXISTS(SELECT 1 FROM Users WHERE Id = 'USR-CUST-02')
    INSERT INTO Users (Id, RoleId, FullName, Email, Password, PhoneNumber, IsActive) 
    VALUES ('USR-CUST-02', 'ROLE-CUST', N'Lê Văn Khách', 'cust2@gmail.com', '123456', '0999888777', 1);

-- Seed Vehicles
IF NOT EXISTS(SELECT 1 FROM CustomerVehicles WHERE Id = 'VEH-01')
    INSERT INTO CustomerVehicles (Id, CustomerId, LicensePlate, VehicleModel, Color)
    VALUES ('VEH-01', 'USR-CUST-01', '51A-123.45', 'Toyota Vios', 'Trắng');

IF NOT EXISTS(SELECT 1 FROM CustomerVehicles WHERE Id = 'VEH-02')
    INSERT INTO CustomerVehicles (Id, CustomerId, LicensePlate, VehicleModel, Color)
    VALUES ('VEH-02', 'USR-CUST-02', '30F-567.89', 'Honda Civic', 'Đen');

IF NOT EXISTS(SELECT 1 FROM CustomerVehicles WHERE Id = 'VEH-03')
    INSERT INTO CustomerVehicles (Id, CustomerId, LicensePlate, VehicleModel, Color)
    VALUES ('VEH-03', 'USR-CUST-01', '51H-999.99', 'Mazda CX5', 'Đỏ');

-- Seed Wash Services
IF NOT EXISTS(SELECT 1 FROM WashServices WHERE Id = 'SRV-01')
    INSERT INTO WashServices (Id, ServiceName, Description, Price, DurationMinutes, ServiceType, IsActive)
    VALUES ('SRV-01', N'Rửa xe tiêu chuẩn', N'Rửa ngoài bọt tuyết', 50000, 30, 'Standard', 1);

-- Seed Bookings
-- 1. Pending (Hôm nay, sắp tới)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-01')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-01', 'USR-CUST-01', 'USR-STAFF-01', 'VEH-01', CAST(GETDATE() AS DATE), DATEADD(HOUR, 2, GETDATE()), DATEADD(HOUR, 3, GETDATE()), 'Pending', 50000, 0);

-- 2. Confirmed (Đã xác nhận, sắp tới)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-02')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-02', 'USR-CUST-02', 'USR-STAFF-01', 'VEH-02', CAST(GETDATE() AS DATE), DATEADD(MINUTE, 35, GETDATE()), DATEADD(MINUTE, 65, GETDATE()), 'Confirmed', 150000, 0);

-- 3. InProgress (Đang xử lý, vừa qua giờ)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-03')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-03', 'USR-CUST-01', 'USR-STAFF-01', 'VEH-03', CAST(GETDATE() AS DATE), DATEADD(MINUTE, -10, GETDATE()), DATEADD(MINUTE, 20, GETDATE()), 'InProgress', 200000, 0);

-- 4. Completed (Đã xong hôm qua)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-04')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-04', 'USR-CUST-02', 'USR-STAFF-01', 'VEH-02', CAST(GETDATE()-1 AS DATE), DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, -1, DATEADD(HOUR, 1, GETDATE())), 'Completed', 120000, 0);

-- 5. Completed (Đã xong 2 ngày trước)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-05')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-05', 'USR-CUST-01', 'USR-STAFF-01', 'VEH-01', CAST(GETDATE()-2 AS DATE), DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, -2, DATEADD(HOUR, 1, GETDATE())), 'Completed', 300000, 0);

-- 6. Pending but within 30 mins (Cho test thử nút Đổi Trạng Thái)
IF NOT EXISTS(SELECT 1 FROM Bookings WHERE Id = 'BKG-TEST-06')
    INSERT INTO Bookings (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate, ScheduledStartTime, ScheduledEndTime, Status, TotalPrice, IsDeleted)
    VALUES ('BKG-TEST-06', 'USR-CUST-02', 'USR-STAFF-01', 'VEH-02', CAST(GETDATE() AS DATE), DATEADD(MINUTE, 20, GETDATE()), DATEADD(MINUTE, 50, GETDATE()), 'Pending', 100000, 0);

-- Seed Service Reviews (Feedback) cho 2 đơn đã completed
IF NOT EXISTS(SELECT 1 FROM ServiceReviews WHERE Id = 'RVW-01')
    INSERT INTO ServiceReviews (Id, BookingId, CustomerId, OverallRating, Comment, CreatedAt)
    VALUES ('RVW-01', 'BKG-TEST-04', 'USR-CUST-02', 5, N'Dịch vụ rất tốt, nhân viên nhiệt tình, rửa xe sạch bong!', DATEADD(DAY, -1, GETDATE()));

IF NOT EXISTS(SELECT 1 FROM ServiceReviews WHERE Id = 'RVW-02')
    INSERT INTO ServiceReviews (Id, BookingId, CustomerId, OverallRating, Comment, CreatedAt)
    VALUES ('RVW-02', 'BKG-TEST-05', 'USR-CUST-01', 4, N'Rửa cũng sạch nhưng hôm đó trời mưa nên xe lại bẩn, hơi đen. Thái độ tốt.', DATEADD(DAY, -2, GETDATE()));

GO
