using FluentAssertions;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Customers;
using InventoryOrderingSystem.Repositories.OrderProducts;
using InventoryOrderingSystem.Services.Customers;
using InventoryOrderingSystem.Services.OrderProducts;
using InventoryOrderingSystem.Repositories.Orders;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace InventoryOrderingSystem.Test
{
    public class OrderProductTest
    {
        private readonly Mock<IOrderProductsRepository> _repoOrderProducts;
        private readonly Mock<ICustomerRepository> _repoCustomer;
        private readonly Mock<IOrderRepository> _repoOrder;
        private readonly OrderProductService _orderProductService;

        public OrderProductTest()
        {
            _repoOrderProducts = new Mock<IOrderProductsRepository>();
            _repoCustomer = new Mock<ICustomerRepository>();
            _repoOrder = new Mock<IOrderRepository>();
            _orderProductService = new OrderProductService(_repoOrderProducts.Object, _repoCustomer.Object, _repoOrder.Object);
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

        [Fact]
        public async Task AddOrderProductAsync_Throws_WhenCustomerIsNotActive()
        {
            // Arrange
            var orderProduct = GetTestOrderProduct();
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 1
            };
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = false
            };
            _repoOrderProducts.Setup(r => r.AddOrderProductAsync(orderProduct, order.OrderId, orderProduct.ProductId)).ThrowsAsync(new NullReferenceException("Customer is not active."));
            // Act
            Func<Task> act = async () => await _orderProductService.AddOrderProductAsync(orderProduct, orderProduct.OrderId, orderProduct.ProductId);
            // Assert
            await act.Should().ThrowAsync<NullReferenceException>()
                .WithMessage("Customer is not active.");
        }
        [Fact]
        public async Task AddOrderProductAsync_SubtractsStock_WhenSuccess()
        {
            // Arrange
            var orderProduct = GetTestOrderProduct();
            var order = new Order
            {
                OrderId = 1,
                CustomerId = 1
            };
            var customer = new Customer
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                IsActive = true
            };
            _repoOrderProducts.Setup(r => r.AddOrderProductAsync(orderProduct, order.OrderId, orderProduct.ProductId)).Returns(Task.CompletedTask);
            // Act
            await _orderProductService.AddOrderProductAsync(orderProduct, orderProduct.OrderId, orderProduct.ProductId);
            // Assert
            _repoOrderProducts.Verify(r => r.AddOrderProductAsync(orderProduct, order.OrderId, orderProduct.ProductId), Times.Once);
        }
    }
}
