using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Customers;
using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Services.Admins;
using InventoryOrderingSystem.Services.OrderProducts;
using InventoryOrderingSystem.Services.Orders;
using InventoryOrderingSystem.Services.Products;

namespace InventoryOrderingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderService _orderService;
        private readonly IOrderProductService _orderProductService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IAdminService _adminService;

        public HomeController(
            IOrderService orderService,
            IOrderProductService orderProductService,
            IProductService productService,
            ICustomerService customerService,
            IAdminService adminService)
        {
            _orderService = orderService;
            _orderProductService = orderProductService;
            _productService = productService;
            _customerService = customerService;
            _adminService = adminService;
        }

        public async Task<IActionResult> Index()
        {
            // Redirect if not logged in
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Role")))
                return RedirectToAction("Login");

            var role = HttpContext.Session.GetString("Role");

            if (role == "Admin")
            {
                var products = await _productService.GetAllProductsAsync();
                var customers = await _customerService.GetAllCustomersAsync();
                var orders = await _orderService.GetAllOrdersAsync();

                ViewBag.TotalProducts = products.Count();
                ViewBag.TotalCustomers = customers.Count(c => c.IsActive);
                ViewBag.PendingOrders = orders.Count(o => o.Status == "Pending");

                var lowStock = products.Where(p => p.Stock < 10).OrderBy(p => p.Stock).ToList();
                var recentOrders = orders.Take(5).ToList();

                return View("AdminDashboard", lowStock);
            }

            // --- ADDED FOR CUSTOMER ROLE ---
            if (role == "Customer")
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                var allOrders = await _orderService.GetAllOrdersAsync();

                // Filter orders so they only see their own
                var myOrders = allOrders.Where(o => o.CustomerId == userId).ToList();

                return View("CustomerDashboard", myOrders);
            }

            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // This wipes the UserId and Role
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
        {
            var existingCustomer = _customerService.GetCustomerByEmail(email);
            if (existingCustomer == null)
            {
                var customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = password,
                    IsActive = true
                };

                // CRITICAL: You MUST await this call
                await _customerService.AddCustomerAsync(customer);

                return RedirectToAction("Login");
            }

            ViewBag.Error = "Email already exists.";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // 1. Check Admins first
            var admin = _adminService.GetAdminByEmail(email);
            if (admin != null)
            {
                // For testing, if you haven't hashed the DB password yet, 
                // you might need: if (password == admin.Password)
                if (SecurityHelper.VerifyPassword(password.Trim(), admin.Password))
                {
                    HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                    HttpContext.Session.SetString("Role", "Admin");
                    return RedirectToAction("Index", "Home");
                }
            }

            // 2. Check Customers
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer != null)
            {
                string cleanedStoredHash = customer.Password.Trim();
                if (SecurityHelper.VerifyPassword(password.Trim(), cleanedStoredHash))
                {
                    HttpContext.Session.SetInt32("UserId", customer.CustomerId);
                    HttpContext.Session.SetString("Role", "Customer");
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }
    }
}
