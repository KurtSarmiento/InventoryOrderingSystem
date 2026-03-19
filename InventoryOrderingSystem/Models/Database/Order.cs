using System;
using System.Collections.Generic;

namespace InventoryOrderingSystem.Models.Database;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string Status { get; set; } = null!;

    public DateOnly DateCreated { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
