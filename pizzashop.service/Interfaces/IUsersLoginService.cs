using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IUsersLoginService
{
    Task<Userslogin> GetUserByEmail(string email);

    Task<Userslogin> GetUserById(int id);

    Task<(List<Userslogin> users, int totalUsers, int totalPages)> GetPaginatedUsersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder);

    Task UpdateUserLoginData(UserViewModel model);

    bool CheckUsername(string Username , int? Id = null);

    bool CheckEmail(string Email, int? Id = null);
}
