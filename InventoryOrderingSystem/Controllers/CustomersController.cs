using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Customers;

namespace InventoryOrderingSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customers (Requirement #5)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Home");

            var customers = await _customerService.GetAllCustomersAsync();

            // 1. Search Logic (Search by Name or Email)
            if (!string.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(c =>
                    c.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 2. Pagination Logic (10 per page)
            int pageSize = 10;
            int totalItems = customers.Count();
            var pagedData = customers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchString = searchString;

            return View(pagedData);
        }

        // Action for the Details button
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login", "Home");

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null) return NotFound();

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Home");
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        public async Task<IActionResult> Create(string firstName, string lastName, string email, string password)
        {
            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password, // Remember: Your Service handles the hashing!
                IsActive = true
            };

            await _customerService.AddCustomerAsync(customer);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login", "Home");

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer != null)
            {
                // Flip the bit: if true, make false. If false, make true.
                customer.IsActive = !customer.IsActive;
                await _customerService.UpdateCustomerAsync(customer);
            }

            return RedirectToAction("Details", new { id = id });
        }
    }
}