using pizzashop.repository.Models;

namespace pizzashop.service.Interfaces;

public interface IUsersLoginService
{
    Task<Userslogin> GetUserByEmail(string email);

    Task<Userslogin> GetUserById(int id);
}
