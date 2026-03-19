using InventoryOrderingSystem.Models.Database;
using InventoryOrderingSystem.Repositories.Admins;

namespace InventoryOrderingSystem.Services.Admins
{
    public class AdminService(IAdminRepository repo) : IAdminService
    {
        private readonly IAdminRepository _repo = repo;
        public async Task<Admin> LoginAsync(string username, string password)
        {
            var admin = await _repo.GetByUsernameAsync(username)
                ?? throw new NullReferenceException("Admin not found.");

            if (!admin.IsActive)
                throw new InvalidOperationException("Admin is inactive.");

            if (admin.Password != password)
                throw new UnauthorizedAccessException("Invalid password.");

            return admin;
        }
    }
}
