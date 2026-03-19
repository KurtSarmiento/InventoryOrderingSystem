using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repositories.Admins
{
    public interface IAdminRepository
    {
        Task<Admin?> GetByUsernameAsync(string username);
        Task AddAdminAsync(Admin admin);
    }
}
