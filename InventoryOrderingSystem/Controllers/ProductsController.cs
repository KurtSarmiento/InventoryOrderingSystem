using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Products;
using Microsoft.AspNetCore.Mvc;

namespace InventoryOrderingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // List all products
        public async Task<IActionResult> Index()
        {
            // Security check
            if (HttpContext.Session.GetString("Role") != "Admin") return RedirectToAction("Login", "Home");

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // Add new product (GET)
        public IActionResult Create() => View();

        // Add new product (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
