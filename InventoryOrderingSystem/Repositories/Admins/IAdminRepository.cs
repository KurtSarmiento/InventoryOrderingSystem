using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Repositories.Admins
{
    public interface IAdminRepository
    {
        Admin GetByEmail(string email);
    }
}
