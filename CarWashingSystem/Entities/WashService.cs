using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class WashService
{
    public string Id { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public string ServiceType { get; set; } = null!;

    public decimal Price { get; set; }

    public int DurationMinutes { get; set; }

    public string? IconName { get; set; }

    public bool IsPopular { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
