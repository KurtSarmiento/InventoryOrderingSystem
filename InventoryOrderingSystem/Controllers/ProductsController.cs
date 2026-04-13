using Microsoft.AspNetCore.Mvc;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Services.Products;

namespace InventoryOrderingSystem.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products (Requirement #4)
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Home");

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Home");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        public async Task<IActionResult> Create(string productCode, string name, decimal price, int stock)
        {
            var product = new Product
            {
                ProductCode = productCode,
                Name = name,
                Price = price,
                Stock = stock
            };

            await _productService.AddProductAsync(product);
            return RedirectToAction("Index");
        }
    }
}