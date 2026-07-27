/* =============================================================
   CarWashingSystemDB - FILE DUY NHAT
   Tao database + tao bang + do du lieu mau day du.

   Luu y: file nay XOA database cu roi tao lai tu dau.
   Chay tren: (local) hoac .\SQLEXPRESS

   Cac bang: Roles, Users, CustomerVehicles, WashServices,
             Bookings, BookingServices, Invoices, ServiceReviews
   (Da bo hoan toan bang Branches va cot BranchId)

   Tai khoan mau - mat khau tat ca deu la 123456:
     staff1@lunawash.com     - Nhan vien
     staff2@lunawash.com     - Nhan vien
     customer@lunawash.com   - Khach hang (dung cho man Booking)
     customer2@lunawash.com  - Khach hang
     customer3@lunawash.com  - Khach hang
   ============================================================= */

USE master;
GO

IF DB_ID('CarWashingSystemDB') IS NOT NULL
BEGIN
    ALTER DATABASE CarWashingSystemDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE CarWashingSystemDB;
END
GO

CREATE DATABASE CarWashingSystemDB;
GO

USE CarWashingSystemDB;
GO


/* =============================================================
   PHAN 1: TAO BANG
   ============================================================= */

/* ---------- Roles: phan quyen Staff / Customer ---------- */
CREATE TABLE dbo.Roles (
    Id          VARCHAR(50)    NOT NULL PRIMARY KEY,
    RoleName    NVARCHAR(50)   NOT NULL,
    Description NVARCHAR(250)  NULL,
    CreatedAt   DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt   DATETIME2      NULL,
    IsDeleted   BIT            NOT NULL DEFAULT 0
);
GO

/* ---------- Users: dung cho Login / Register / Profile / Staff ---------- */
CREATE TABLE dbo.Users (
    Id          VARCHAR(50)    NOT NULL PRIMARY KEY,
    FullName    NVARCHAR(150)  NOT NULL,
    Email       NVARCHAR(150)  NOT NULL,
    PhoneNumber NVARCHAR(20)   NOT NULL,
    Password    NVARCHAR(250)  NULL,
    RoleId      VARCHAR(50)    NOT NULL,
    Address     NVARCHAR(255)  NULL,
    AvatarUrl   NVARCHAR(500)  NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2      NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt   DATETIME2      NULL,
    IsDeleted   BIT            NOT NULL DEFAULT 0,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

/* ---------- CustomerVehicles: xe cua khach (Profile them/sua/xoa) ---------- */
CREATE TABLE dbo.CustomerVehicles (
    Id            VARCHAR(50)   NOT NULL PRIMARY KEY,
    CustomerId    VARCHAR(50)   NOT NULL,
    LicensePlate  NVARCHAR(50)  NOT NULL,
    VehicleModel  NVARCHAR(100) NOT NULL,
    Color         NVARCHAR(50)  NULL,
    CreatedAt     DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    IsDeleted     BIT           NOT NULL DEFAULT 0,   -- xoa xe = xoa mem
    CONSTRAINT FK_CustomerVehicles_Users FOREIGN KEY (CustomerId) REFERENCES dbo.Users(Id)
);
GO

/* ---------- WashServices: goi dich vu + dich vu them ---------- */
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

/* ---------- Bookings: dat lich + lich su ---------- */
CREATE TABLE dbo.Bookings (
    Id                 VARCHAR(50)   NOT NULL PRIMARY KEY,
    CustomerId         VARCHAR(50)   NOT NULL,
    AssignedStaffId    VARCHAR(50)   NULL,     -- nhan vien duoc giao viec
    CustomerVehicleId  VARCHAR(50)   NULL,
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
    CONSTRAINT FK_Bookings_Users_Staff      FOREIGN KEY (AssignedStaffId)   REFERENCES dbo.Users(Id),
    CONSTRAINT FK_Bookings_CustomerVehicles FOREIGN KEY (CustomerVehicleId) REFERENCES dbo.CustomerVehicles(Id)
);
GO

/* Man Staff loc theo AssignedStaffId + thoi gian */
CREATE INDEX IX_Bookings_AssignedStaffId
    ON dbo.Bookings (AssignedStaffId, ScheduledStartTime DESC);
GO

/* ---------- BookingServices: bang noi booking <-> dich vu ---------- */
CREATE TABLE dbo.BookingServices (
    BookingId VARCHAR(50) NOT NULL,
    ServiceId VARCHAR(50) NOT NULL,
    CONSTRAINT PK_BookingServices PRIMARY KEY (BookingId, ServiceId),
    CONSTRAINT FK_BookingServices_Bookings     FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(Id),
    CONSTRAINT FK_BookingServices_WashServices FOREIGN KEY (ServiceId) REFERENCES dbo.WashServices(Id)
);
GO

/* ---------- Invoices: hoa don thanh toan ---------- */
CREATE TABLE dbo.Invoices (
    Id             VARCHAR(50)   NOT NULL PRIMARY KEY,
    BookingId      VARCHAR(50)   NOT NULL,
    OriginalAmount DECIMAL(18,2) NOT NULL,
    DiscountAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    FinalAmount    DECIMAL(18,2) NOT NULL,
    PaymentMethod  NVARCHAR(50)  NOT NULL,   -- Cash / Momo / Card
    PaymentStatus  NVARCHAR(50)  NOT NULL DEFAULT N'Unpaid', -- Unpaid / Paid / Refunded
    PaymentTime    DATETIME2     NULL,
    CreatedAt      DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
    UpdatedAt      DATETIME2     NULL,
    IsDeleted      BIT           NOT NULL DEFAULT 0,
    CONSTRAINT FK_Invoices_Bookings FOREIGN KEY (BookingId) REFERENCES dbo.Bookings(Id)
);
GO

/* ---------- ServiceReviews: khach danh gia, staff tra loi ---------- */
CREATE TABLE dbo.ServiceReviews (
    Id                VARCHAR(50)    NOT NULL PRIMARY KEY,
    BookingId         VARCHAR(50)    NOT NULL,
    CustomerId        VARCHAR(50)    NOT NULL,
    OverallRating     FLOAT          NOT NULL,
    CleanlinessRating INT            NOT NULL DEFAULT 0,
    SpeedRating       INT            NOT NULL DEFAULT 0,
    StaffRating       INT            NOT NULL DEFAULT 0,
    Comment           NVARCHAR(1000) NULL,
    ResponseText      NVARCHAR(1000) NULL,    -- staff tra loi
    RespondedById     VARCHAR(50)    NULL,
    RespondedAt       DATETIME2      NULL,
    CreatedAt         DATETIME2      NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_ServiceReviews_Bookings          FOREIGN KEY (BookingId)     REFERENCES dbo.Bookings(Id),
    CONSTRAINT FK_ServiceReviews_Users             FOREIGN KEY (CustomerId)    REFERENCES dbo.Users(Id),
    CONSTRAINT FK_ServiceReviews_Users_RespondedBy FOREIGN KEY (RespondedById) REFERENCES dbo.Users(Id)
);
GO


/* =============================================================
   PHAN 2: DU LIEU MAU
   ============================================================= */

/* ---------- Roles (2) ---------- */
INSERT INTO dbo.Roles (Id, RoleName, Description) VALUES
(N'ROL-STAFF', N'Staff',    N'Nhan vien rua xe'),
(N'ROL-CUST',  N'Customer', N'Khach hang su dung dich vu');
GO

/* ---------- Users (2 staff + 3 khach) - mat khau: 123456 ---------- */
INSERT INTO dbo.Users (Id, FullName, Email, PhoneNumber, Password, RoleId, Address) VALUES
(N'USR-STAFF-01', N'Nguyễn Văn Sáng', N'staff1@lunawash.com',    N'0911000001', N'123456', N'ROL-STAFF', N'12 Lê Lợi, Quận 1'),
(N'USR-STAFF-02', N'Trần Thị Mai',    N'staff2@lunawash.com',    N'0911000002', N'123456', N'ROL-STAFF', N'45 Cộng Hòa, Tân Bình'),
(N'USR-CUST-01',  N'Lê Minh Khang',   N'customer@lunawash.com',  N'0922000001', N'123456', N'ROL-CUST',  N'88 Nguyễn Huệ, Quận 1'),
(N'USR-CUST-02',  N'Phạm Thu Hà',     N'customer2@lunawash.com', N'0922000002', N'123456', N'ROL-CUST',  N'23 Phan Xích Long, Phú Nhuận'),
(N'USR-CUST-03',  N'Đỗ Quốc Bảo',     N'customer3@lunawash.com', N'0922000003', N'123456', N'ROL-CUST',  N'7 Nguyễn Văn Linh, Quận 7');
GO

/* ---------- CustomerVehicles (7, trong do 1 xe da xoa mem) ---------- */
INSERT INTO dbo.CustomerVehicles (Id, CustomerId, LicensePlate, VehicleModel, Color, IsDeleted) VALUES
(N'VEH-CUST01-01', N'USR-CUST-01', N'51A-123.45',  N'Toyota Vios',   N'Trắng', 0),
(N'VEH-CUST01-02', N'USR-CUST-01', N'51G-678.90',  N'Honda CR-V',    N'Đen',   0),
(N'VEH-CUST01-03', N'USR-CUST-01', N'59F1-234.56', N'Ford Ranger',   N'Xám',   0),
(N'VEH-CUST01-04', N'USR-CUST-01', N'51B-000.11',  N'Kia Morning',   N'Đỏ',    1),  -- da xoa mem, KHONG hien tren Booking
(N'VEH-CUST02-01', N'USR-CUST-02', N'30A-555.66',  N'Mazda CX-5',    N'Xanh',  0),
(N'VEH-CUST02-02', N'USR-CUST-02', N'30F-777.88',  N'Hyundai Accent',N'Bạc',   0),
(N'VEH-CUST03-01', N'USR-CUST-03', N'92A-321.65',  N'Mitsubishi Xpander', N'Nâu', 0);
GO

/* ---------- WashServices (10, trong do 1 goi ngung ban) ---------- */
INSERT INTO dbo.WashServices (Id, ServiceName, Description, ServiceType, Price, DurationMinutes, IconName, IsPopular, IsActive) VALUES
-- 2 goi dung cho 2 RadioButton o man Booking
(N'SRV-STANDARD', N'Rửa tiêu chuẩn', N'Rửa sạch ngoại thất, làm khô tự động.',              N'Package',  50000.00, 30, N'water_drop',  0, 1),
(N'SRV-PREMIUM',  N'Rửa cao cấp',    N'Rửa toàn diện, vệ sinh nội thất và phủ bóng sơn.',   N'Package', 100000.00, 45, N'diamond',     1, 1),
-- cac goi khac
(N'SRV-BASIC',    N'Cơ Bản',         N'Rửa sạch ngoại thất, làm khô tự động.',              N'Package', 149000.00, 15, N'water_drop',  0, 1),
(N'SRV-ADVANCED', N'Nâng cao',       N'Dịch vụ cơ bản kết hợp vệ sinh gầm và tẩy ố lazang.',N'Package', 249000.00, 20, N'cool_to_dry', 1, 1),
(N'SRV-DELUXE',   N'Cao cấp',        N'Rửa xe toàn diện với phủ Nano Ceramic bảo vệ sơn.',  N'Package', 499000.00, 30, N'diamond',     0, 1),
(N'SRV-OLDPACK',  N'Gói cũ ngừng bán', N'Gói đã ngừng kinh doanh.',                         N'Package', 300000.00, 40, N'block',       0, 0),  -- IsActive = 0
-- dich vu them
(N'SRV-SEAT',     N'Vệ Sinh Đệm Ghế Da', N'Làm sạch sâu và dưỡng mềm ghế da.',              N'AddOn',   500000.00, 60, N'airline_seat_recline_normal', 0, 1),
(N'SRV-GLASS',    N'Tẩy Ố Mốc Kính', N'Tẩy cặn canxi, ố mốc lâu ngày trên kính xe.',        N'AddOn',   250000.00, 30, N'cleaning_services', 0, 1),
(N'SRV-NANO',     N'Phủ Nano Kính',  N'Phủ Nano kính lái và kính sườn, chống bám nước mưa.',N'AddOn',   350000.00, 30, N'blur_on',     0, 1),
(N'SRV-OIL',      N'Thay Dầu / Nhớt',N'Thay nhớt động cơ, kiểm tra lốp và lọc nhớt.',       N'AddOn',   450000.00, 30, N'oil_barrel',  1, 1);
GO

/* ---------- Bookings (8, du cac trang thai) ---------- */
INSERT INTO dbo.Bookings
    (Id, CustomerId, AssignedStaffId, CustomerVehicleId, BookingDate,
     ScheduledStartTime, ScheduledEndTime, Status, CheckInTime, CheckoutTime, TotalPrice, Notes)
VALUES
-- Da hoan thanh, da thanh toan, da co danh gia
(N'BKG-0001', N'USR-CUST-01', N'USR-STAFF-01', N'VEH-CUST01-01', '2026-07-20',
 '2026-07-20T09:00:00', '2026-07-20T09:30:00', N'Completed', '2026-07-20T08:55:00', '2026-07-20T09:35:00',  50000.00, N'Khách quen'),

(N'BKG-0002', N'USR-CUST-01', N'USR-STAFF-02', N'VEH-CUST01-02', '2026-07-22',
 '2026-07-22T14:00:00', '2026-07-22T14:45:00', N'Completed', '2026-07-22T13:58:00', '2026-07-22T14:50:00', 100000.00, NULL),

(N'BKG-0003', N'USR-CUST-02', N'USR-STAFF-01', N'VEH-CUST02-01', '2026-07-24',
 '2026-07-24T10:00:00', '2026-07-24T11:00:00', N'Completed', '2026-07-24T09:57:00', '2026-07-24T11:05:00', 350000.00, N'Có mua thêm phủ nano'),

-- Da thanh toan, cho toi ngay lam
(N'BKG-0004', N'USR-CUST-02', N'USR-STAFF-02', N'VEH-CUST02-02', '2026-07-30',
 '2026-07-30T08:30:00', '2026-07-30T09:15:00', N'Confirmed', NULL, NULL, 100000.00, NULL),

-- Dang rua
(N'BKG-0005', N'USR-CUST-03', N'USR-STAFF-01', N'VEH-CUST03-01', '2026-07-27',
 '2026-07-27T15:00:00', '2026-07-27T15:30:00', N'InProgress', '2026-07-27T14:58:00', NULL, 50000.00, NULL),

-- CHUA THANH TOAN - dung 2 don nay de test man Payment
(N'BKG-0006', N'USR-CUST-01', NULL, N'VEH-CUST01-03', '2026-07-29',
 '2026-07-29T09:30:00', '2026-07-29T10:00:00', N'Pending', NULL, NULL,  50000.00, NULL),

(N'BKG-0007', N'USR-CUST-03', NULL, N'VEH-CUST03-01', '2026-07-31',
 '2026-07-31T16:00:00', '2026-07-31T16:45:00', N'Pending', NULL, NULL, 100000.00, N'Gọi trước 15 phút'),

-- Da huy
(N'BKG-0008', N'USR-CUST-02', N'USR-STAFF-02', N'VEH-CUST02-01', '2026-07-18',
 '2026-07-18T11:00:00', '2026-07-18T11:30:00', N'Cancelled', NULL, NULL, 50000.00, N'Khách bận đột xuất');
GO

/* ---------- BookingServices: dich vu cua tung don ---------- */
INSERT INTO dbo.BookingServices (BookingId, ServiceId) VALUES
(N'BKG-0001', N'SRV-STANDARD'),
(N'BKG-0002', N'SRV-PREMIUM'),
(N'BKG-0003', N'SRV-PREMIUM'),
(N'BKG-0003', N'SRV-NANO'),      -- don nay mua them phu nano
(N'BKG-0004', N'SRV-PREMIUM'),
(N'BKG-0005', N'SRV-STANDARD'),
(N'BKG-0006', N'SRV-STANDARD'),
(N'BKG-0007', N'SRV-PREMIUM'),
(N'BKG-0008', N'SRV-STANDARD');
GO

/* ---------- Invoices: chi cac don da thanh toan / dang cho ---------- */
INSERT INTO dbo.Invoices
    (Id, BookingId, OriginalAmount, DiscountAmount, FinalAmount, PaymentMethod, PaymentStatus, PaymentTime)
VALUES
(N'INV-0001', N'BKG-0001',  50000.00,     0.00,  50000.00, N'Cash', N'Paid',   '2026-07-20T09:36:00'),
(N'INV-0002', N'BKG-0002', 100000.00,     0.00, 100000.00, N'Momo', N'Paid',   '2026-07-22T14:51:00'),
(N'INV-0003', N'BKG-0003', 350000.00, 50000.00, 300000.00, N'Card', N'Paid',   '2026-07-24T11:06:00'),
(N'INV-0004', N'BKG-0004', 100000.00,     0.00, 100000.00, N'Momo', N'Paid',   '2026-07-25T20:10:00'),
(N'INV-0005', N'BKG-0005',  50000.00,     0.00,  50000.00, N'Cash', N'Unpaid', NULL);
GO

/* ---------- ServiceReviews: danh gia cua khach ---------- */
INSERT INTO dbo.ServiceReviews
    (Id, BookingId, CustomerId, OverallRating, CleanlinessRating, SpeedRating, StaffRating,
     Comment, ResponseText, RespondedById, RespondedAt, CreatedAt)
VALUES
(N'REV-0001', N'BKG-0001', N'USR-CUST-01', 5.0, 5, 5, 5,
 N'Xe sạch, nhân viên thân thiện. Rất hài lòng!',
 N'Cảm ơn anh đã tin tưởng dịch vụ của LunaWash ạ!', N'USR-STAFF-01', '2026-07-20T18:00:00', '2026-07-20T10:00:00'),

(N'REV-0002', N'BKG-0002', N'USR-CUST-01', 4.0, 4, 3, 5,
 N'Rửa kỹ nhưng chờ hơi lâu.',
 NULL, NULL, NULL, '2026-07-22T15:30:00'),

(N'REV-0003', N'BKG-0003', N'USR-CUST-02', 4.5, 5, 4, 4,
 N'Phủ nano rất đẹp, sẽ quay lại.',
 N'Cảm ơn chị, hẹn gặp lại ạ!', N'USR-STAFF-01', '2026-07-25T09:00:00', '2026-07-24T12:00:00');
GO


/* =============================================================
   PHAN 3: KIEM TRA
   ============================================================= */

SELECT 'Roles' AS Bang, COUNT(*) AS SoDong FROM dbo.Roles
UNION ALL SELECT 'Users',            COUNT(*) FROM dbo.Users
UNION ALL SELECT 'CustomerVehicles', COUNT(*) FROM dbo.CustomerVehicles
UNION ALL SELECT 'WashServices',     COUNT(*) FROM dbo.WashServices
UNION ALL SELECT 'Bookings',         COUNT(*) FROM dbo.Bookings
UNION ALL SELECT 'BookingServices',  COUNT(*) FROM dbo.BookingServices
UNION ALL SELECT 'Invoices',         COUNT(*) FROM dbo.Invoices
UNION ALL SELECT 'ServiceReviews',   COUNT(*) FROM dbo.ServiceReviews;
GO

/* Don cho thanh toan - dung de test man Payment */
SELECT b.Id, u.FullName, v.LicensePlate, b.ScheduledStartTime, b.Status, b.TotalPrice
FROM dbo.Bookings b
JOIN dbo.Users u ON u.Id = b.CustomerId
LEFT JOIN dbo.CustomerVehicles v ON v.Id = b.CustomerVehicleId
WHERE b.Status = N'Pending'
ORDER BY b.ScheduledStartTime;
GO
