using FluentAssertions;
using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Customers;
using InventoryOrderingSystem.Services.Customers;
using Microsoft.AspNetCore.Identity;
using Moq;
namespace InventoryOrderingSystem.Test
{
    public class CustomerServiceTest
    {
        private readonly Mock<ICustomerRepository> _repo;
        private readonly CustomerService _customerService;

        public CustomerServiceTest()
        {
            _repo = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_repo.Object);
        }

        private static Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                IsActive = false
            };
        }

        [Fact]
        public async Task AddCustomerAsync_Throws_WhenEmailAlreadyExists()
        {
            // Arrange
            var customer = GetTestCustomer();
            _repo.Setup(r => r.GetCustomerByEmailAsync(customer.Email)).ReturnsAsync(customer);

            // Act
            Func<Task> act = async () => await _customerService.AddCustomerAsync(customer);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Email already exists.");
        }

    }
}
