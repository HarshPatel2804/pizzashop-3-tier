using Microsoft.AspNetCore.Http;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(int id);
    Task DeleteUser(int id);

    Task AddUser(UserViewModel model , IFormFile ProfileImg);

    Task<UserViewModel> GetUserData(int id);

    Task UpdateUserData(UserViewModel model , IFormFile ProfileImg);

    bool CheckPhone(string Phone, int? Id = null);
}
