using InventoryOrderingSystem.Models.Database;

namespace InventoryOrderingSystem.Services.Admins
{
    public interface IAdminService
    {
        Admin GetAdminByEmail(string email);
    }
}
