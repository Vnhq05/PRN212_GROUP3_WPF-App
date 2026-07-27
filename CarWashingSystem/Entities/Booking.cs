using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class Booking
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string BranchId { get; set; } = null!;

    public string? AssignedStaffId { get; set; }

    public string? CustomerVehicleId { get; set; }

    public DateOnly BookingDate { get; set; }

    public DateTime ScheduledStartTime { get; set; }

    public DateTime ScheduledEndTime { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckoutTime { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User? AssignedStaff { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User Customer { get; set; } = null!;

    public virtual CustomerVehicle? CustomerVehicle { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();

    public virtual ICollection<WashService> Services { get; set; } = new List<WashService>();
}
