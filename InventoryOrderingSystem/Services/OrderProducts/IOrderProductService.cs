using InventoryOrderingSystem.Models.Database;
namespace InventoryOrderingSystem.Services.OrderProducts
{
    public interface IOrderProductService
    {
        Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId);
        Task DeleteOrderProductAsync(int id);
        Task<List<OrderProduct>> GetAllOrderProductsAsync();
        Task<OrderProduct> GetOrderProductByIdAsync(int id);
        Task UpdateOrderProductAsync(OrderProduct orderProduct);
    }
}
