using InventoryOrderingSystem.Models.Database;
namespace InventoryOrderingSystem.Repositories.OrderProducts
{
    public interface IOrderProductsRepository
    {
        Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId);
        Task DeleteOrderProductAsync(int id);
        Task<List<OrderProduct>> GetAllOrderProductsAsync();
        Task<OrderProduct> GetOrderProductByIdAsync(int id);
        Task UpdateOrderProductAsync(OrderProduct orderProduct);
    }
}
