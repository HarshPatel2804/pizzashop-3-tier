using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class UserRepository : IUserRepository
{
     private readonly PizzaShopContext _context;

    public UserRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<User> AddUser(UserViewModel model)
    {
         var user = new User
        {
            Firstname = model.FirstName,
            Lastname = model.LastName,
            Phone = model.Phone,
            Countryid = model.CountryId,
            Stateid = model.StateId,
            Cityid = model.CityId,
            Address = model.Address,
            Zipcode = model.Zipcode
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Userid == id);
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }
}
