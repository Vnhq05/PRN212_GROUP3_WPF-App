using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class CustomerVehicle
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public string VehicleModel { get; set; } = null!;

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User Customer { get; set; } = null!;
}
