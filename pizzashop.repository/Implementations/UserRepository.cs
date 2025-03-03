using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class UserRepository : IUserRepository
{
     private readonly PizzaShopContext _context;

    public UserRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Userid == id);
    }

}
