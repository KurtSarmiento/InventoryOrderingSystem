using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Customers;

namespace InventoryOrderingSystem.Services.Customers
{
    public class CustomerService(ICustomerRepository customerRepository) : ICustomerService
    {
        private readonly ICustomerRepository _repo = customerRepository;
        public async Task AddCustomerAsync(Customer customer)
        {
            var existingCustomer = await _repo.GetCustomerByEmailAsync(customer.Email);
            if (existingCustomer != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }
            await _repo.AddCustomerAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _repo.DeleteCustomerAsync(id);
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _repo.GetAllCustomersAsync();
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            return await _repo.GetCustomerByEmailAsync(email);
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _repo.GetCustomerByIdAsync(id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await _repo.UpdateCustomerAsync(customer);
        }
    }
}
