USE CarWashingSystemDB;
GO

UPDATE Users SET FullName = N'Trần Văn Nhân Viên' WHERE Id = 'USR-STAFF-01';
UPDATE Users SET FullName = N'Nguyễn Khách Hàng' WHERE Id = 'USR-CUST-01';
UPDATE Users SET FullName = N'Lê Văn Khách' WHERE Id = 'USR-CUST-02';

UPDATE CustomerVehicles SET Color = N'Trắng' WHERE Id = 'VEH-01';
UPDATE CustomerVehicles SET Color = N'Đen' WHERE Id = 'VEH-02';
UPDATE CustomerVehicles SET Color = N'Đỏ' WHERE Id = 'VEH-03';

UPDATE WashServices SET ServiceName = N'Rửa xe tiêu chuẩn', Description = N'Rửa ngoài bọt tuyết' WHERE Id = 'SRV-01';

UPDATE ServiceReviews SET Comment = N'Dịch vụ rất tốt, nhân viên nhiệt tình, rửa xe sạch bong!' WHERE Id = 'RVW-01';
UPDATE ServiceReviews SET Comment = N'Rửa cũng sạch nhưng hôm đó trời mưa nên xe lại bẩn, hơi đen. Thái độ tốt.' WHERE Id = 'RVW-02';
GO
