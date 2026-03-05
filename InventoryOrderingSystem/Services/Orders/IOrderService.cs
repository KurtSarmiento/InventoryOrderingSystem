using InventoryOrderingSystem.Models.Database;
namespace InventoryOrderingSystem.Services.Orders
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order, int customerId);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
    }
}
