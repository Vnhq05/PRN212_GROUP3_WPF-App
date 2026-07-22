/* =============================================================
   CarWashingSystemDB - trích xuất từ LunaWashDB (Azure)
   Chỉ gồm các bảng cần cho các màn hình:
   Login, Register, Customer Profile (Add/Update/Delete car),
   Booking, Payment, History, Feedback, Staff (History, Feedback)
   Chạy trên: localhost\SQLEXPRESS
   ============================================================= */

IF DB_ID('CarWashingSystemDB') IS NULL
    CREATE DATABASE CarWashingSystemDB;
GO

USE CarWashingSystemDB;
GO

/* ---------- Roles (Login/Register - phân quyền Customer/Staff) ---------- */
IF OBJECT_ID('dbo.Roles') IS NULL
CREATE TABLE dbo.Roles (
    Id          VARCHAR(50)    NOT NULL PRIMARY KEY,
    RoleName    NVARCHAR(50)   NOT NULL,
    Description NVARCHAR(250)  NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt   DATETIME2      NULL,
    IsDeleted   BIT            NOT NULL DEFAULT 0
);
GO

/* ---------- Branches (chi nhánh - Booking cần chọn chi nhánh) ---------- */
IF OBJECT_ID('dbo.Branches') IS NULL
CREATE TABLE dbo.Branches (
    Id          VARCHAR(50)    NOT NULL PRIMARY KEY,
    BranchName  NVARCHAR(150)  NOT NULL,
    Address     NVARCHAR(250)  NOT NULL,
    PhoneNumber NVARCHAR(20)   NOT NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    Description NVARCHAR(500)  NULL,
    ImageUrl    NVARCHAR(MAX)  NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt   DATETIME2      NULL,
    IsDeleted   BIT            NOT NULL DEFAULT 0
);
GO

/* ---------- Users (Login / Register / Customer Profile / Staff) ---------- */
IF OBJECT_ID('dbo.Users') IS NULL
CREATE TABLE dbo.Users (
    Id          VARCHAR(50)    NOT NULL PRIMARY KEY,
    FullName    NVARCHAR(150)  NOT NULL,
    Email       NVARCHAR(150)  NOT NULL,
    PhoneNumber NVARCHAR(20)   NOT NULL,
    Password    NVARCHAR(250)  NULL,
    RoleId      VARCHAR(50)    NOT NULL,
    BranchId    VARCHAR(50)    NULL,          -- chi nhánh làm việc (dành cho Staff)
    Address     NVARCHAR(255)  NULL,
    AvatarUrl   NVARCHAR(500)  NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt   DATETIME2      NULL,
    IsDeleted   BIT            NOT NULL DEFAULT 0,
    CONSTRAINT FK_Users_Roles    FOREIGN KEY (RoleId)   REFERENCES dbo.Roles(Id),
    CONSTRAINT FK_Users_Branches FOREIGN KEY (BranchId) REFERENCES dbo.Branches(Id),
    CONSTRAINT UQ_Users_Email    UNIQUE (Email)
);
GO

/* ---------- CustomerVehicles (Add/Update/Delete car ở Customer Profile) ---------- */
IF OBJECT_ID('dbo.CustomerVehicles') IS NULL
CREATE TABLE dbo.CustomerVehicles (
    Id            VARCHAR(50)   NOT NULL PRIMARY KEY,
    CustomerId    VARCHAR(50)   NOT NULL,
    LicensePlate  NVARCHAR(50)  NOT NULL,
    VehicleModel  NVARCHAR(100) NOT NULL,
    Color         NVARCHAR(50)  NULL,
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    IsDeleted     BIT           NOT NULL DEFAULT 0,   -- Delete car = soft delete
    CONSTRAINT FK_CustomerVehicles_Users FOREIGN KEY (CustomerId) REFERENCES dbo.Users(Id)
);
GO

/* ---------- WashServices (dịch vụ rửa xe - chọn khi Booking, giá gộp thẳng vào đây) ---------- */
IF OBJECT_ID('dbo.WashServices') IS NULL
CREATE TABLE dbo.WashServices (
    Id              VARCHAR(50)   NOT NULL PRIMARY KEY,
    ServiceName     NVARCHAR(150) NOT NULL,
    Description     NVARCHAR(500) NULL,
    ServiceType     NVARCHAR(50)  NOT NULL DEFAULT N'Package',  -- Package | AddOn
    Price           DECIMAL(18,2) NOT NULL DEFAULT 0,
    DurationMinutes INT           NOT NULL DEFAULT 30,
    IconName        NVARCHAR(100) NULL,
    IsPopular       BIT           NOT NULL DEFAULT 0,
    IsActive        BIT           NOT NULL DEFAULT 1,
    CreatedAt       DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt       DATETIME2     NULL,
    IsDeleted       BIT           NOT NULL DEFAULT 0
);
GO

/* ---------- Bookings (Booking + History) ---------- */
IF OBJECT_ID('dbo.Bookings') IS NULL
CREATE TABLE dbo.Bookings (
    Id                 VARCHAR(50)   NOT NULL PRIMARY KEY,
    CustomerId         VARCHAR(50)   NOT NULL,
    BranchId           VARCHAR(50)   NOT NULL,
    CustomerVehicleId  VARCHAR(50)   NULL,     -- xe cụ thể của khách (để History hiển thị xe)
    BookingDate        DATE          NOT NULL,
    ScheduledStartTime DATETIME2     NOT NULL,
    ScheduledEndTime   DATETIME2     NOT NULL,
    Status             NVARCHAR(50)  NOT NULL DEFAULT N'Pending', -- Pending/Confirmed/InProgress/Completed/Cancelled
    CheckInTime        DATETIME2     NULL,
    CheckoutTime       DATETIME2     NULL,
    TotalPrice         DECIMAL(18,2) NOT NULL DEFAULT 0,
    Notes              NVARCHAR(MAX) NULL,
    CreatedAt          DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt          DATETIME2     NULL,
    IsDeleted          BIT           NOT NULL DEFAULT 0,
    CONSTRAINT FK_Bookings_Users            FOREIGN KEY (CustomerId)        REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Bookings_Branches         FOREIGN KEY (BranchId)          REFERENCES dbo.Branches(Id),
    CONSTRAINT FK_Bookings_CustomerVehicles FOREIGN KEY (CustomerVehicleId) REFERENCES dbo.CustomerVehicles(Id)
);
GO

/* ---------- BookingServices (các dịch vụ đã chọn trong 1 booking) ---------- */
IF OBJECT_ID('dbo.BookingServices') IS NULL
CREATE TABLE dbo.BookingServices (
    BookingId VARCHAR(50) NOT NULL,
    ServiceId VARCHAR(50) NOT NULL,
    CONSTRAINT PK_BookingServices PRIMARY KEY (BookingId, ServiceId),
    CONSTRAINT FK_BookingServices_Bookings     FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(Id),
    CONSTRAINT FK_BookingServices_WashServices FOREIGN KEY (ServiceId) REFERENCES dbo.WashServices(Id)
);
GO

/* ---------- Invoices (Payment) ---------- */
IF OBJECT_ID('dbo.Invoices') IS NULL
CREATE TABLE dbo.Invoices (
    Id             VARCHAR(50)   NOT NULL PRIMARY KEY,
    BookingId      VARCHAR(50)   NOT NULL,
    OriginalAmount DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    FinalAmount    DECIMAL(18,2) NOT NULL,
    PaymentMethod  NVARCHAR(50)  NOT NULL,   -- Cash / Card / Momo / VNPay ...
    PaymentStatus  NVARCHAR(50)  NOT NULL DEFAULT N'Unpaid', -- Unpaid / Paid / Refunded
    PaymentTime    DATETIME2     NULL,
    CreatedAt      DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt      DATETIME2     NULL,
    IsDeleted      BIT           NOT NULL DEFAULT 0,
    CONSTRAINT FK_Invoices_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(Id)
);
GO

/* ---------- ServiceReviews (Feedback: khách gửi, staff xem & trả lời) ---------- */
IF OBJECT_ID('dbo.ServiceReviews') IS NULL
CREATE TABLE dbo.ServiceReviews (
    Id                VARCHAR(50)    NOT NULL PRIMARY KEY,
    BookingId         VARCHAR(50)    NOT NULL,
    CustomerId        VARCHAR(50)    NOT NULL,
    BranchId          VARCHAR(50)    NOT NULL,
    OverallRating     FLOAT          NOT NULL,
    CleanlinessRating INT            NOT NULL DEFAULT 0,
    SpeedRating       INT            NOT NULL DEFAULT 0,
    StaffRating       INT            NOT NULL DEFAULT 0,
    Comment           NVARCHAR(1000) NULL,
    ResponseText      NVARCHAR(1000) NULL,    -- staff trả lời feedback
    RespondedById     VARCHAR(50)    NULL,
    RespondedAt       DATETIME2      NULL,
    CreatedAt         DATETIME2      NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_ServiceReviews_Bookings          FOREIGN KEY (BookingId)     REFERENCES dbo.Bookings(Id),
    CONSTRAINT FK_ServiceReviews_Users             FOREIGN KEY (CustomerId)    REFERENCES dbo.Users(Id),
    CONSTRAINT FK_ServiceReviews_Branches          FOREIGN KEY (BranchId)      REFERENCES dbo.Branches(Id),
    CONSTRAINT FK_ServiceReviews_Users_RespondedBy FOREIGN KEY (RespondedById) REFERENCES dbo.Users(Id)
);
GO

/* Dữ liệu tham chiếu + tài khoản mẫu: chạy tiếp file 02_SeedData.sql */

/* Seed data trich xuat tu LunaWashDB (Azure) - generated 2026-07-22 */
USE CarWashingSystemDB;
GO

/* ---------- Roles (5 rows) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Roles)
INSERT INTO dbo.Roles (Id, RoleName, Description) VALUES
(N'ROL-01', N'Admin', N'Quan tri vien toan he thong'),
(N'ROL-02', N'Staff', N'Nhan vien chi nhanh ho tro check-in'),
(N'ROL-03', N'Customer', N'Khach hang su dung dich vu'),
(N'ROL-04', N'BranchManager', NULL),
(N'ROL-05', N'TechnicalStaff', N'Technical and maintenance staff');
GO

/* ---------- Branches (5 rows) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Branches)
INSERT INTO dbo.Branches (Id, BranchName, Address, PhoneNumber, IsActive, Description, ImageUrl) VALUES
(N'BRN-LD-01', N'LunaWash Linh Đông', N'Thủ Đức, HCM', N'0900000001', 1, NULL, NULL),
(N'BRN-Q1-01', N'LunaWash Quận 1', N'123 Lê Lợi, Bến Thành', N'02838383838', 1, NULL, NULL),
(N'BRN-Q7-01', N'LunaWash Quận 7', N'456 Nguyễn Văn Linh', N'0900000003', 1, NULL, NULL),
(N'BRN-TB-01', N'LunaWash Tân Bình', N'789 Cộng Hòa, Phường 13', N'0900000004', 1, NULL, NULL),
(N'BRN-TTH-01', N'LunaWash Tân Thới Hiệp', N'Quận 12, HCM', N'0900000002', 1, NULL, NULL);
GO

/* ---------- WashServices (7 rows - gia lay theo bang gia o to 4 cho cua LunaWashDB) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.WashServices)
INSERT INTO dbo.WashServices (Id, ServiceName, Description, ServiceType, Price, DurationMinutes, IconName, IsPopular, IsActive) VALUES
(N'SRV-03A4A5FB', N'Vệ Sinh Đệm Ghế Da', N'Sử dụng dung dịch chuyên dụng làm sạch sâu và dưỡng mềm ghế da.', N'AddOn', 500000.00, 60, N'airline_seat_recline_normal', 0, 1),
(N'SRV-6A03D6A1', N'Cơ Bản', N'Rửa sạch ngoại thất , làm khô tự động', N'Package', 149000.00, 15, N'water_drop', 0, 1),
(N'SRV-8155020C', N'Tẩy Ố Mốc Kính', N'Tẩy sạch cặn canxi, ố mốc lâu ngày trên kính xe, trả lại sự trong suốt.', N'AddOn', 250000.00, 30, N'cleaning_services', 0, 1),
(N'SRV-A065CEEF', N'Nâng cao', N'Dịch vụ cơ bản kết hợp vệ sinh gầm và tẩy ố Lazang', N'Package', 249000.00, 20, N'cool_to_dry', 1, 1),
(N'SRV-BD30884F', N'Cao cấp', N'Rủa xe toàn diện với phủ Nano Creramic bảo vệ sơn xe', N'Package', 499000.00, 30, N'diamond', 0, 1),
(N'SRV-E71A770B', N'Phủ Nano Kính', N'Phủ Nano kính lái và kính sườn, hiệu ứng lá sen chống bám nước mưa.', N'AddOn', 350000.00, 30, N'blur_on', 0, 1),
(N'SRV-E7442245', N'Thay Dầu / Nhớt', N'Thay nhớt động cơ cao cấp, kiểm tra lốp và làm sạch lọc nhớt.', N'AddOn', 450000.00, 30, N'oil_barrel', 1, 1);
GO

/* ---------- Tai khoan mau de test Login ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Users)
INSERT INTO dbo.Users (Id, FullName, Email, PhoneNumber, Password, RoleId, BranchId)
VALUES
('USR-ADMIN-01', N'Quản trị viên',    'admin@lunawash.com',    '0900000001', N'123456', 'ROL-01', NULL),
('USR-STAFF-01', N'Nhân viên Quận 1', 'staff@lunawash.com',    '0900000002', N'123456', 'ROL-02', 'BRN-Q1-01'),
('USR-CUST-01',  N'Khách hàng Demo',  'customer@lunawash.com', '0900000003', N'123456', 'ROL-03', NULL);
GO
