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


    public async Task<(List<Userslogin> users, int totalUsers)> GetPaginatedUsersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder)
    {
        var query = _context.Userslogins
                    .Include(u => u.User)
                    .Include(u => u.Role)
                    .Where(u => u.User.Isdeleted != true)
                    .Where(u => string.IsNullOrEmpty(search) ||
                        u.User.Firstname.ToLower().Contains(search.ToLower()) ||
                        u.User.Lastname.ToLower().Contains(search.ToLower()) ||
                        u.Email.ToLower().Contains(search.ToLower()) ||
                        u.User.Phone.ToLower().Contains(search.ToLower()) ||
                        u.Role.Rolename.ToLower().Contains(search.ToLower()));


        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortOrder))
        {
            switch (sortColumn)
            {
                case "Name":
                    query = sortOrder == "asc"
                        ? query.OrderBy(u => u.User.Firstname)
                        : query.OrderByDescending(u => u.User.Firstname);
                    break;
                case "Role":
                    query = sortOrder == "asc"
                        ? query.OrderBy(u => u.Role.Rolename)
                        : query.OrderByDescending(u => u.Role.Rolename);
                    break;
                default:
                    query = query.OrderBy(u => u.Userloginid);
                    break;
            }
        }

        int totalUsers = await query.CountAsync();

        var users = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (users, totalUsers);
    }

    public async Task<Userslogin> AddUserloginDetails(UserViewModel model, int id)
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
