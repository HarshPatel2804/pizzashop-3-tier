using pizzashop.repository.Models;

namespace pizzashop.service.Interfaces;

public interface IUsersLoginService
{
    Task<Userslogin> GetUserByEmail(string email);

    Task<Userslogin> GetUserById(int id);

    Task<(List<Userslogin> users, int totalUsers, int totalPages)> GetPaginatedUsersAsync(int page, int pageSize, string search);
}
