using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class User
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Password { get; set; }

    public string RoleId { get; set; } = null!;

    public string? BranchId { get; set; }

    public string? Address { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<CustomerVehicle> CustomerVehicles { get; set; } = new List<CustomerVehicle>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<ServiceReview> ServiceReviewCustomers { get; set; } = new List<ServiceReview>();

    public virtual ICollection<ServiceReview> ServiceReviewRespondedBies { get; set; } = new List<ServiceReview>();
}
