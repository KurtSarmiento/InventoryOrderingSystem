using InventoryOrderingSystem.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryOrderingSystem.Repositories.Admins
{
    public class AdminRepository(InventoryOrderingSystemContext context) : IAdminRepository
    {
        private readonly InventoryOrderingSystemContext _context = context;

        public Admin GetByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(x => x.Email == email);
        }
    }
}
