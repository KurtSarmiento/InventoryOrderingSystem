using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.OrderProducts;
using InventoryOrderingSystem.Repositories.Customers;
using InventoryOrderingSystem.Repositories.Orders;
using InventoryOrderingSystem.Repositories.Products;

namespace InventoryOrderingSystem.Services.OrderProducts
{
    public class OrderProductService(
        IOrderProductsRepository orderProductsRepository,
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository) : IOrderProductService
    {
        private readonly IOrderProductsRepository _repoOrderProducts = orderProductsRepository;
        private readonly ICustomerRepository _repoCustomer = customerRepository;
        private readonly IOrderRepository _repoOrder = orderRepository;
        private readonly IProductRepository _repoProduct = productRepository;

        public async Task AddOrderProductAsync(OrderProduct orderProduct, int orderId, int productId)
        {
            var order = await _repoOrder.GetOrderByIdAsync(orderId)
                ?? throw new NullReferenceException("Order does not exist.");

            var customer = await _repoCustomer.GetCustomerByIdAsync(order.CustomerId)
                ?? throw new NullReferenceException("Customer does not exist.");

            if (!customer.IsActive)
                throw new InvalidOperationException("Customer is not active.");

            var product = await _repoProduct.GetProductByIdAsync(productId)
                ?? throw new NullReferenceException("Product does not exist.");

            if (orderProduct.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0.");

            if (product.Stock < orderProduct.Quantity)
                throw new InvalidOperationException("Insufficient product stock.");

            orderProduct.TotalAmount = product.Price * orderProduct.Quantity;

            orderProduct.OrderId = orderId;
            orderProduct.ProductId = productId;

            await _repoOrderProducts.AddOrderProductAsync(orderProduct, orderId, productId);

            product.Stock -= orderProduct.Quantity;
            await _repoProduct.UpdateProductAsync(product);
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