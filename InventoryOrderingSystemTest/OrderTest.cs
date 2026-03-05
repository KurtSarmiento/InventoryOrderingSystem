using InventoryOrderingSystem.Repositories.Orders;
using InventoryOrderingSystem.Services.Orders;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryOrderingSystem.Test
{
    public class OrderTest
    {
        private readonly Mock<IOrderRepository> _repo;
        private readonly OrderService _orderService;
    }
}
