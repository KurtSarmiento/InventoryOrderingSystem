using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Products;
namespace InventoryOrderingSystem.Services.Products
{
    public class ProductService(IProductRepository productRepository) : IProductService
    {
        private readonly IProductRepository _repo = productRepository;
        public async Task AddProductAsync(Product product)
        {
            await _repo.AddProductAsync(product);
        }
        public async Task DeleteProductAsync(int id)
        {
            await _repo.DeleteProductAsync(id);
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _repo.GetAllProductsAsync();
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _repo.GetProductByIdAsync(id);
        }
        public async Task UpdateProductAsync(Product product)
        {
            await _repo.UpdateProductAsync(product);
        }
    }
}
