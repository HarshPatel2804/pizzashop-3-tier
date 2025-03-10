using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

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

    public async Task UpdateUserLoginDetails(Userslogin user)
    {
       _context.Userslogins.Update(user);
       _context.SaveChanges();
    }


     public async Task<(List<Userslogin> users, int totalUsers)> GetPaginatedUsersAsync(int page, int pageSize, string search)
        {
            var query = _context.Userslogins
                .Include(u => u.User)
                .Include(u => u.Role)
                .Where(u => u.User.Isdeleted != true)
                .Where(u => string.IsNullOrEmpty(search) ||
                            u.User.Firstname.Contains(search) ||
                            u.User.Lastname.Contains(search) ||
                            u.Email.Contains(search) ||
                            u.User.Phone.Contains(search) ||
                            u.Role.Rolename.Contains(search));

            int totalUsers = await query.CountAsync();

            var users = await query
                .OrderBy(u => u.Userloginid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalUsers);
        }

    public async Task<Userslogin> AddUserloginDetails(UserViewModel model , int id)
    {
        var userLogin = new Userslogin
        {
            Email = model.Email,
            Passwordhash = model.Password,
            Userid = id,
            Username = model.Username,
            Roleid = model.Role
        };

        await _context.Userslogins.AddAsync(userLogin);
        await _context.SaveChangesAsync();
        return userLogin;
    }

}
