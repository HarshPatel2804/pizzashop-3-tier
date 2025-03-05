using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IUserRepository
{
     Task<User> GetUserByIdAsync(int id);

     Task UpdateUser(User user);

     Task<User> AddUser(UserViewModel model);

     Task<UserViewModel> GetUserDataAsync(int id);
}
