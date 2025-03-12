using Microsoft.AspNetCore.Mvc.Rendering;
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
            Zipcode = model.Zipcode,
            Profileimg = model.Profileimg
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

    public async Task<UserViewModel> GetUserDataAsync(int id){
        var userLoginDetails = _context.Userslogins.FirstOrDefault(u => u.Userid == id);
        var user = _context.Users.FirstOrDefault(u => u.Userid == id);
        var role = _context.Roles.FirstOrDefault(r => r.Roleid == userLoginDetails.Roleid);

        var model = new UserViewModel
        {
            Id = id,
            FirstName = user.Firstname,
            LastName = user.Lastname,
            Email = userLoginDetails.Email,
            Username = userLoginDetails.Username,
            Role = role.Roleid,
            Phone = user.Phone,
            Address = user.Address,
            Zipcode = user.Zipcode,
            CountryId = user.Countryid,
            StateId = user.Stateid,
            CityId = user.Cityid,
            Profileimg = user.Profileimg,
            status = (ViewModels.statustype)userLoginDetails.status,
            Roles = _context.Roles.Select(r => new SelectListItem
            {
                Value = r.Roleid.ToString(),
                Text = r.Rolename
            }).ToList(),
            Countries = _context.Countries.Select(c => new SelectListItem
            {
                Value = c.Countryid.ToString(),
                Text = c.Countryname
            }).ToList(),
            States = _context.States.Where(c => c.Countryid == user.Countryid).Select(c => new SelectListItem
            {
                Value = c.Stateid.ToString(),
                Text = c.Statename
            }).ToList(),
            Cities = _context.Cities.Where(c => c.Stateid == user.Stateid).Select(c => new SelectListItem
            {
                Value = c.Cityid.ToString(),
                Text = c.Cityname
            }).ToList()
        };
        return model;
    }
}
