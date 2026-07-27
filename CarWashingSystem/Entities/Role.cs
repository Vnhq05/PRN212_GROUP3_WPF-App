using System;
using System.Collections.Generic;

namespace CarWashingSystem.Entities;

public partial class Role
{
    public string Id { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
