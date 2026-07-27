using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class CarWashingSystemDbContext : DbContext
{
    public CarWashingSystemDbContext()
    {
    }

    public CarWashingSystemDbContext(DbContextOptions<CarWashingSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<CustomerVehicle> CustomerVehicles { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ServiceReview> ServiceReviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WashService> WashServices { get; set; }

    protected override void OnConfiguring(
    DbContextOptionsBuilder optionsBuilder)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: true)
            .Build();

        string? strConn = config["ConnectionStrings:DefaultConnection"];

        optionsBuilder.UseSqlServer(strConn);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC0716A9C57D");

            entity.HasIndex(e => new { e.AssignedStaffId, e.ScheduledStartTime }, "IX_Bookings_AssignedStaffId").IsDescending(false, true);

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.AssignedStaffId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CustomerVehicleId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.AssignedStaff).WithMany(p => p.BookingAssignedStaffs)
                .HasForeignKey(d => d.AssignedStaffId)
                .HasConstraintName("FK_Bookings_Users_Staff");

            entity.HasOne(d => d.Branch).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Branches");

            entity.HasOne(d => d.Customer).WithMany(p => p.BookingCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Users");

            entity.HasOne(d => d.CustomerVehicle).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CustomerVehicleId)
                .HasConstraintName("FK_Bookings_CustomerVehicles");

            entity.HasMany(d => d.Services).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingService",
                    r => r.HasOne<WashService>().WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookingServices_WashServices"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookingServices_Bookings"),
                    j =>
                    {
                        j.HasKey("BookingId", "ServiceId");
                        j.ToTable("BookingServices");
                        j.IndexerProperty<string>("BookingId")
                            .HasMaxLength(50)
                            .IsUnicode(false);
                        j.IndexerProperty<string>("ServiceId")
                            .HasMaxLength(50)
                            .IsUnicode(false);
                    });
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branches__3214EC07C271A3E3");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(250);
            entity.Property(e => e.BranchName).HasMaxLength(150);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<CustomerVehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC075A6E3F6F");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LicensePlate).HasMaxLength(50);
            entity.Property(e => e.VehicleModel).HasMaxLength(100);

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerVehicles)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerVehicles_Users");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Invoices__3214EC07FDA4AE7C");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FinalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OriginalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Unpaid");

            entity.HasOne(d => d.Booking).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Bookings");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0732D591FE");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<ServiceReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ServiceR__3214EC074058BFE9");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BookingId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RespondedById)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ResponseText).HasMaxLength(1000);

            entity.HasOne(d => d.Booking).WithMany(p => p.ServiceReviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceReviews_Bookings");

            entity.HasOne(d => d.Branch).WithMany(p => p.ServiceReviews)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceReviews_Branches");

            entity.HasOne(d => d.Customer).WithMany(p => p.ServiceReviewCustomers)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceReviews_Users");

            entity.HasOne(d => d.RespondedBy).WithMany(p => p.ServiceReviewRespondedBies)
                .HasForeignKey(d => d.RespondedById)
                .HasConstraintName("FK_ServiceReviews_Users_RespondedBy");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0790601188");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.BranchId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(250);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_Users_Branches");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<WashService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WashServ__3214EC077A4AFFEB");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DurationMinutes).HasDefaultValue(30);
            entity.Property(e => e.IconName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(150);
            entity.Property(e => e.ServiceType)
                .HasMaxLength(50)
                .HasDefaultValue("Package");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
