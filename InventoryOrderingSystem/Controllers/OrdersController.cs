using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Orders;
using InventoryOrderingSystem.Services.OrderProducts;
using InventoryOrderingSystem.Services.Products; // Add this
using InventoryOrderingSystem.Services.Customers; // Add this
using InventoryOrderingSystem.Models;

namespace InventoryOrderingSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;
        private readonly IProductService _productService; // Add this
        private readonly ICustomerService _customerService; // Add this

        public OrdersController(
            IOrderService orderService,
            IOrderProductService orderProductService,
            IProductService productService,
            ICustomerService customerService)
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
            _productService = productService;
            _customerService = customerService;
        }

        // GET: /Orders/Index
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Home");

            if (role == "Customer") return RedirectToAction("Index", "Home");

            // 1. Fetch orders AND Include the Customer data (Fixes "Unknown Customer")
            var ordersQuery = await _orderService.GetAllOrdersAsync();
            var orders = ordersQuery.AsEnumerable();

            // 2. Search Logic
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o =>
                    o.OrderId.ToString().Contains(searchString) ||
                    (o.Customer != null && (
                        o.Customer.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                        o.Customer.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                    ))
                );
            }

            // 3. Pagination Logic (10 per page)
            int pageSize = 10;
            int totalItems = orders.Count();
            var pagedData = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchString = searchString;

            return View(pagedData);
        }

        // GET: /Orders/Create
        public async Task<IActionResult> Create()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId"); // Get UserId for status check
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Home");

            // --- ADDED: Block Inactive Customers from accessing the form ---
            if (role == "Customer")
            {
                var customer = await _customerService.GetCustomerByIdAsync(userId ?? 0);
                if (customer == null || !customer.IsActive)
                {
                    TempData["ErrorMessage"] = "Your account is currently inactive. You cannot place new orders.";
                    return RedirectToAction("Index", "Home");
                }
            }

            var products = await _productService.GetAllProductsAsync();
            var model = new OrderVM
            {
                AllProducts = products.ToList()
            };

            if (role == "Admin")
            {
                var customers = await _customerService.GetAllCustomersAsync();
                model.AllCustomers = customers.Where(c => c.IsActive).ToList();
            }
            else if (role == "Customer")
            {
                // Auto-assign the CustomerId from the session
                model.CustomerId = HttpContext.Session.GetInt32("UserId") ?? 0;

                // We don't need the list of all customers for a regular user
                model.AllCustomers = new List<Customer>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderVM model)
        {
            // 1. Basic Security & Validation
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId"); // Get UserId for server-side check
            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Home");

            // --- ADDED: Server-side check to prevent forced POST from inactive users ---
            if (role == "Customer")
            {
                var customer = await _customerService.GetCustomerByIdAsync(userId ?? 0);
                if (customer == null || !customer.IsActive)
                {
                    return Unauthorized();
                }
            }

            ModelState.Remove("AllCustomers");
            ModelState.Remove("AllProducts");

            if (!ModelState.IsValid)
            {
                // Reload dropdowns if validation fails
                model.AllProducts = await _productService.GetAllProductsAsync();
                if (role == "Admin") model.AllCustomers = await _customerService.GetAllCustomersAsync();
                return View(model);
            }

            // 2. Create the Parent Order
            var order = new Order
            {
                CustomerId = model.CustomerId,
                Status = "Pending",
                DateCreated = DateOnly.FromDateTime(DateTime.Now)
            };
            await _orderService.AddOrderAsync(order);

            // 3. The Critical Part: Saving each Item
            if (model.Items != null && model.Items.Any())
            {
                foreach (var item in model.Items)
                {
                    // Skip rows that were added but no product was selected
                    if (item.ProductId == 0 || item.Quantity <= 0) continue;

                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.OrderId, // Use the ID from the order we just saved
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        IsDelivered = false,
                        DateOrdered = DateOnly.FromDateTime(DateTime.Now)
                    };

                    await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, item.ProductId);
                }
            }

            // Redirect based on role
            return role == "Admin" ? RedirectToAction("Index") : RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login", "Home");

            // 1. Get the order including the items so we know what to put back
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order != null && order.Status == "Pending")
            {
                // 2. If we are cancelling, replenish the stock
                if (status == "Cancelled")
                {
                    foreach (var item in order.OrderProducts)
                    {
                        var product = await _productService.GetProductByIdAsync(item.ProductId);
                        if (product != null)
                        {
                            // Add the ordered quantity back to the current stock
                            product.Stock += item.Quantity;
                            await _productService.UpdateProductAsync(product);
                        }
                    }
                }

                // 3. Update the order status itself
                order.Status = status;
                await _orderService.UpdateOrderAsync(order);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(role)) return RedirectToAction("Login", "Home");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            // SECURITY CHECK: If user is a Customer, they can ONLY see their own order
            if (role == "Customer" && order.CustomerId != userId)
            {
                return Unauthorized();
            }

            // Calculation logic for Total Amount
            decimal total = 0;
            foreach (var op in order.OrderProducts)
            {
                total += op.Quantity * op.Product.Price;
            }
            ViewBag.TotalAmount = total;

            return View(order);
        }
    }
}