using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repositories.Admins
{
    public class AdminRepository(InventoryOrderingSystemContext context) : IAdminRepository
    {
        private readonly InventoryOrderingSystemContext _context = context;
        public async Task AddAdminAsync(Admin admin)
        {
            await _context.Admins.AddAsync(admin);
            await _context.SaveChangesAsync();
        }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Username == username);
        }
    }
}
