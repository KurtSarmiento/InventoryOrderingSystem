using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Admins;

namespace InventoryOrderingSystem.Services.Admins
{
    public class AdminService(IAdminRepository repo) : IAdminService
    {
        private readonly IAdminRepository _repo = repo;

        public Admin GetAdminByEmail(string email)
        {
            return _repo.GetByEmail(email);
        }
    }
}
