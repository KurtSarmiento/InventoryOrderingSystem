using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.OrderProducts;

namespace InventoryOrderingSystem.Services.OrderProducts
{
    public class OrderProductService(IOrderProductsRepository orderProductsRepository) : IOrderProductService
    {
        private readonly IOrderProductsRepository _repo = orderProductsRepository;
        public async Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId)
        {
            await _repo.AddOrderProductAsync(orderProduct, orderId, productId);
        }

        public async Task DeleteOrderProductAsync(int id)
        {
            await _repo.DeleteOrderProductAsync(id);
        }

        public async Task<List<OrderProduct>> GetAllOrderProductsAsync()
        {
            return await _repo.GetAllOrderProductsAsync();
        }

        public async Task<OrderProduct> GetOrderProductByIdAsync(int id)
        {
            return await _repo.GetOrderProductByIdAsync(id);
        }

        public async Task UpdateOrderProductAsync(OrderProduct orderProduct)
        {
            await _repo.UpdateOrderProductAsync(orderProduct);
        }
    }
}
