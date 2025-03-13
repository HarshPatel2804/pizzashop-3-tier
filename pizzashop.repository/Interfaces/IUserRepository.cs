using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IUserRepository
{
     Task<User> GetUserByIdAsync(int id);

     Task UpdateUser(User user);

     Task<User> AddUser(UserViewModel model);

     bool CheckPhone(string Phone, int? Id = null);

     Task<UserViewModel> GetUserDataAsync(int id);
}
