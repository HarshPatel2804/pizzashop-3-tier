using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class UsersloginRepository : IUsersloginRepository
{ 
        private readonly PizzaShopContext _context;

    public UsersloginRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<Userslogin> GetUserByEmailAsync(string email)
    {
        return await _context.Userslogins
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Userslogin> GetUserByIdAsync(int id)
    {
        return await _context.Userslogins
            .FirstOrDefaultAsync(u => u.Userid == id);
    }

}
