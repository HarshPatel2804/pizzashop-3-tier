using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(int id);
    Task DeleteUser(int id);

    Task AddUser(UserViewModel model);

    Task<UserViewModel> GetUserData(int id);

    Task UpdateUserData(UserViewModel model);
}
