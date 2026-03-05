using InventoryOrderingSystem.Models.Database;
namespace InventoryOrderingSystem.Repositories.Customers
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<Customer> GetCustomerByEmailAsync(string email);
    }
}
