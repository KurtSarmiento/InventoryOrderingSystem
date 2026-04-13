using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Customers;
using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Services.Admins;

namespace InventoryOrderingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IAdminService _adminService;

        public HomeController(ILogger<HomeController> logger, ICustomerService customerService, IAdminService adminService)
            {
                _logger = logger;
                _customerService = customerService;
                _adminService = adminService;
            }
        public string GetHash()
        {
            // This will output a valid hash for '123456' to your browser screen
            return SecurityHelper.HashPassword("123456");
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                // Try to get all customers from the DB
                var customers = await _customerService.GetAllCustomersAsync();

                // Pass the count to the view so we can see it
                ViewBag.DbStatus = "Connected!";
                ViewBag.CustomerCount = customers.Count();
            }
            catch (Exception ex)
            {
                ViewBag.DbStatus = "Error: " + ex.Message;
                ViewBag.CustomerCount = 0;
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
