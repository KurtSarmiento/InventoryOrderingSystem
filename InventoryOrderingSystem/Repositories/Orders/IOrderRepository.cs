using InventoryOrderingSystem.Models.Database;
namespace InventoryOrderingSystem.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task UpdateOrderAsync(Order order);
    }
}
