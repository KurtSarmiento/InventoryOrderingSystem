using FluentAssertions;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Customers;
using InventoryOrderingSystem.Repositories.OrderProducts;
using InventoryOrderingSystem.Services.Customers;
using InventoryOrderingSystem.Services.OrderProducts;
using InventoryOrderingSystem.Repositories.Orders;
using Microsoft.AspNetCore.Identity;
using Moq;
using InventoryOrderingSystem.Repositories.Products;

namespace InventoryOrderingSystem.Test
{
    public class OrderProductTest
    {
        private readonly Mock<IOrderProductsRepository> _repoOrderProducts;
        private readonly Mock<ICustomerRepository> _repoCustomer;
        private readonly Mock<IOrderRepository> _repoOrder;
        private readonly Mock<IProductRepository> _repoProduct;
        private readonly OrderProductService _orderProductService;

        public OrderProductTest()
        {
            _repoOrderProducts = new Mock<IOrderProductsRepository>();
            _repoCustomer = new Mock<ICustomerRepository>();
            _repoOrder = new Mock<IOrderRepository>();
            _repoProduct = new Mock<IProductRepository>();
            _orderProductService = new OrderProductService(_repoOrderProducts.Object, _repoCustomer.Object, _repoOrder.Object, _repoProduct.Object);
        }

        private static OrderProduct GetTestOrderProduct()
        {
            return new OrderProduct
            {
                OrderProductId = 1,
                OrderId = 1,
                ProductId = 1,
                Amount = 20000,
                IsDelivered = false,
                DateOrdered = DateOnly.FromDateTime(DateTime.Now)
            };
        }
        private static Order GetTestOrder()
        {
            return new Order
            {
                OrderId = 1,
                CustomerId = 1
            };
        }

        private static Product GetTestProduct()
        {
            return new Product
            {
                ProductId = 1,
                Name = "Test Product",
                Price = 10000,
                Stock = 10
            };
        }

        private static Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerId = 1,
                FirstName = "Test Customer",
                Email = "test.customer@example.com",
                IsActive = true
            };
        }

        [Fact]
        public async Task AddOrderProductAsync_Throws_WhenUserIsNotActive()
        {
            // Arrange
            var orderProduct = GetTestOrderProduct();
            var order = GetTestOrder();
            var product = GetTestProduct();
            var customer = GetTestCustomer();
            customer.IsActive = false;
            _repoOrder.Setup(x => x.GetOrderByIdAsync(orderProduct.OrderId)).ReturnsAsync(order);
            _repoProduct.Setup(x => x.GetProductByIdAsync(orderProduct.ProductId)).ReturnsAsync(product);
            _repoCustomer.Setup(x => x.GetCustomerByIdAsync(order.CustomerId)).ReturnsAsync(customer);
            // Act
            Func<Task> act = async () => await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, product.ProductId);
            // Assert
            await act.Should().ThrowAsync<NullReferenceException>().WithMessage("Customer is not active.");
        }
        [Fact]
        public async Task AddOrderProductAsync_Throws_WhenProductStockIsZero()
        {
            var orderProduct = GetTestOrderProduct();
            var order = GetTestOrder();
            var product = GetTestProduct();
            var customer = GetTestCustomer();
            product.Stock = 0;
            _repoOrder.Setup(x => x.GetOrderByIdAsync(orderProduct.OrderId)).ReturnsAsync(order);
            _repoProduct.Setup(x => x.GetProductByIdAsync(orderProduct.ProductId)).ReturnsAsync(product);
            _repoCustomer.Setup(x => x.GetCustomerByIdAsync(order.CustomerId)).ReturnsAsync(customer);
            // Act
            Func<Task> act = async () => await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, product.ProductId);
            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Product is out of stock.");
        }
        [Fact]
        public async Task AddOrderProductAsync_ReduceStock_AfterOrder()
        {
            var orderProduct = GetTestOrderProduct();
            var order = GetTestOrder();
            var product = GetTestProduct();
            var customer = GetTestCustomer();
            product.Stock = 10;
            _repoOrder.Setup(x => x.GetOrderByIdAsync(orderProduct.OrderId)).ReturnsAsync(order);
            _repoProduct.Setup(x => x.GetProductByIdAsync(orderProduct.ProductId)).ReturnsAsync(product);
            _repoCustomer.Setup(x => x.GetCustomerByIdAsync(order.CustomerId)).ReturnsAsync(customer);
            // Act
            await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, product.ProductId);
            // Assert
            product.Stock.Should().Be(10 - orderProduct.Quantity);
        }
        [Fact]
        public async Task AddOrderProductAsync_AddsOrderProduct()
        {
            var orderProduct = GetTestOrderProduct();
            var order = GetTestOrder();
            var product = GetTestProduct();
            var customer = GetTestCustomer();
            _repoOrder.Setup(x => x.GetOrderByIdAsync(orderProduct.OrderId)).ReturnsAsync(order);
            _repoProduct.Setup(x => x.GetProductByIdAsync(orderProduct.ProductId)).ReturnsAsync(product);
            _repoCustomer.Setup(x => x.GetCustomerByIdAsync(order.CustomerId)).ReturnsAsync(customer);
            // Act
            await _orderProductService.AddOrderProductAsync(orderProduct, order.OrderId, product.ProductId);
            // Assert
            _repoOrderProducts.Verify(x => x.AddOrderProductAsync(orderProduct, order.OrderId, product.ProductId), Times.Once);

        }
        [Fact]
        public async Task GetOrderProductByIdAsync_ReturnsOrderProduct()
        {
            var orderProduct = GetTestOrderProduct();
            _repoOrderProducts.Setup(x => x.GetOrderProductByIdAsync(orderProduct.OrderProductId)).ReturnsAsync(orderProduct);
            // Act
            var result = await _orderProductService.GetOrderProductByIdAsync(orderProduct.OrderProductId);
            // Assert
            result.Should().BeEquivalentTo(orderProduct);
        }
        [Fact]
        public async Task GetAllOrderProductsAsync_ReturnsListOfOrderProducts()
        {
            var orderProducts = new List<OrderProduct> { GetTestOrderProduct() };
            _repoOrderProducts.Setup(x => x.GetAllOrderProductsAsync()).ReturnsAsync(orderProducts);
            // Act
            var result = await _orderProductService.GetAllOrderProductsAsync();
            // Assert
            result.Should().BeEquivalentTo(orderProducts);
        }
        [Fact]
        public async Task DeleteOrderProductAsync_DeletesOrderProduct()
        {
            var orderProductId = 1;
            // Act
            await _orderProductService.DeleteOrderProductAsync(orderProductId);
            // Assert
            _repoOrderProducts.Verify(x => x.DeleteOrderProductAsync(orderProductId), Times.Once);
        }
    }
}
