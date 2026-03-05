using System;
using System.Collections.Generic;

namespace InventoryOrderingSystem.Models.Database;

public partial class OrderProduct
{
    public int OrderProductId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Amount { get; set; }

    public bool IsDelivered { get; set; }

    public DateOnly DateOrdered { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
