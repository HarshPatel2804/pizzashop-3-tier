using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
namespace pizzashop.repository.Interfaces;

public interface IUsersloginRepository
{
    Task<Userslogin> GetUserByEmailAsync(string email);
    Task<Userslogin> GetUserByIdAsync(int id);

    Task<Userslogin> AddUserloginDetails(UserViewModel model,int id);

    Task UpdateUserLoginDetails(Userslogin user);
    Task<(List<Userslogin> users, int totalUsers)> GetPaginatedUsersAsync(int page, int pageSize, string search);
}
