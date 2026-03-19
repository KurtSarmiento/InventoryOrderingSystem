using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Admins
{
    public interface IAdminService
    {
        Task<Admin> LoginAsync(string username, string password);
    }
}
