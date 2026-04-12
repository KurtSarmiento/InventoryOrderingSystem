using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Orders;
using InventoryOrderingSystem.Services.OrderProducts;
using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;

        public OrdersController(IOrderService orderService, IOrderProductService orderProductService)
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
        }

        // GET: /Orders/Index
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        // GET: /Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // 1. Create the main Order
            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = "Pending",
                DateCreated = DateOnly.FromDateTime(DateTime.Now)
            };

            // 2. Save the Order
            await _orderService.AddOrderAsync(order);

            // 3. Loop through items and save to OrderProduct table
            foreach (var item in model.Items)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    IsDelivered = false,
                    DateOrdered = DateOnly.FromDateTime(DateTime.Now)
                };
                await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, item.ProductId);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}