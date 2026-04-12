using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Orders;

namespace InventoryOrderingSystem.Services.Orders
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService
    {
        private readonly IOrderRepository _repo = orderRepository;
        public async Task AddOrderAsync(Order order)
        {
            await _repo.AddOrderAsync(order);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _repo.DeleteOrderAsync(id);
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _repo.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _repo.GetOrderByIdAsync(id);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _repo.UpdateOrderAsync(order);
        }
    }
}
