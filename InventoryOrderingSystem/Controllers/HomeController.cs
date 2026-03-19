using System.Diagnostics;
using InventoryOrderingSystem.Models;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Customers;
using InventoryOrderingSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Services.Admins;

namespace InventoryOrderingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IAdminService _adminService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string firstName, string lastName, string email, string password)
        {
            var existingCustomer = _customerService.GetCustomerByEmail(email);
            if (existingCustomer == null)
            {
                var customer = new Customer
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Password = SecurityHelper.HashPassword(password)
                };

                _customerService.AddCustomerAsync(customer);
                return View(customer);
            }
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            var admin = _adminService.GetAdminByEmail(email);
            if (customer != null)
            {
                bool isCorrect = SecurityHelper.VerifyPassword(password, customer.Password);
                if (isCorrect)
                {
                    HttpContext.Session.SetInt32("UserId", customer.CustomerId);
                }
                else
                {
                    //retry login
                    return View();
                }
            }
            else if (admin != null)
            {
                bool isCorrect = SecurityHelper.VerifyPassword(password, admin.Password);
                if (isCorrect)
                {
                    HttpContext.Session.SetInt32("UserId", admin.AdminId);
                    return RedirectToAction();
                }
            }
                return View();
        }
    }
}
