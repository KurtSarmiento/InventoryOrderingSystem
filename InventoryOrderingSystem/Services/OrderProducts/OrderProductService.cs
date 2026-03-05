using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.OrderProducts;
using InventoryOrderingSystem.Repositories.Customers;
using InventoryOrderingSystem.Repositories.Orders;

namespace InventoryOrderingSystem.Services.OrderProducts
{
    public class OrderProductService(IOrderProductsRepository orderProductsRepository, ICustomerRepository customerRepository, IOrderRepository orderRepository) : IOrderProductService
    {
        private readonly IOrderProductsRepository _repoOrderProducts = orderProductsRepository;
        private readonly ICustomerRepository _repoCustomer = customerRepository;
        private readonly IOrderRepository _repoOrder = orderRepository;
        public async Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId)
        {
            var order = await _repoOrder.GetOrderByIdAsync(orderProduct.OrderId);
            var customer = await _repoCustomer.GetCustomerByIdAsync(order.CustomerId);
            if (customer == null || customer.IsActive == false)
            {
                throw new NullReferenceException("Customer is not active.");
            }
            await _repoOrderProducts.AddOrderProductAsync(orderProduct, orderId, productId);
        }

        public async Task DeleteOrderProductAsync(int id)
        {
            await _repoOrderProducts.DeleteOrderProductAsync(id);
        }

        public async Task<List<OrderProduct>> GetAllOrderProductsAsync()
        {
            return await _repoOrderProducts.GetAllOrderProductsAsync();
        }

        public async Task<OrderProduct> GetOrderProductByIdAsync(int id)
        {
            return await _repoOrderProducts.GetOrderProductByIdAsync(id);
        }

        public async Task UpdateOrderProductAsync(OrderProduct orderProduct)
        {
            await _repoOrderProducts.UpdateOrderProductAsync(orderProduct);
        }
    }
}
