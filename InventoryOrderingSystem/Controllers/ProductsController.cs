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
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            if (HttpContext.Session.GetString("Role") != "Admin")
                return RedirectToAction("Login", "Home");

            var products = await _productService.GetAllProductsAsync();

            // 1. Enhanced Search Logic
            if (!string.IsNullOrEmpty(searchString))
            {
                string search = searchString.ToLower().Trim();

                if (search == "low stock")
                {
                    // Filter products between 1 and 10
                    products = products.Where(p => p.Stock > 0 && p.Stock <= 10).ToList();
                }
                else if (search == "out of stock")
                {
                    // Filter products with 0 or less
                    products = products.Where(p => p.Stock <= 0).ToList();
                }
                else if (search == "healthy")
                {
                    // Filter products with more than 10
                    products = products.Where(p => p.Stock > 10).ToList();
                }
                else
                {
                    // Default search by Name or ProductCode
                    products = products.Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                                                || p.ProductCode.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            // 2. Pagination Logic (remain same)
            int pageSize = 10;
            int totalItems = products.Count();
            var pagedData = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.SearchString = searchString;

            return View(pagedData);
        }

        // Requirement 4: Update Price and Stock
        [HttpPost]
        public async Task<IActionResult> UpdateInventory(int productId, decimal price, int stock)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product != null)
            {
                product.Price = price;
                product.Stock = stock;
                await _productService.UpdateProductAsync(product);
            }
            return RedirectToAction("Index", new { searchString = HttpContext.Request.Query["searchString"] });
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