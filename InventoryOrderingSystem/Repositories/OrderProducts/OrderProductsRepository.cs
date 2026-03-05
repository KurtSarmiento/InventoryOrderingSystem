using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repositories.OrderProducts
{
    public class OrderProductsRepository(InventoryOrderingSystemContext context) : IOrderProductsRepository
    {
        private readonly InventoryOrderingSystemContext _context = context;
        public async Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId)
        {
            orderProduct.OrderId = orderId;
            orderProduct.ProductId = productId;
            await _context.OrderProducts.AddAsync(orderProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderProductAsync(int id)
        {
            var orderProduct = await _context.OrderProducts.FindAsync(id);
            _context.OrderProducts.Remove(orderProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderProduct>> GetAllOrderProductsAsync()
        {
            return await _context.OrderProducts.ToListAsync();
        }

        public async Task<OrderProduct> GetOrderProductByIdAsync(int id)
        {
            return await _context.OrderProducts.FirstOrDefaultAsync(x => x.OrderProductId == id);
        }

        public async Task UpdateOrderProductAsync(OrderProduct orderProduct)
        {
            _context.OrderProducts.Update(orderProduct);
            await _context.SaveChangesAsync();
        }
    }
}
