using pizzashop.repository.Models;
namespace pizzashop.repository.Interfaces;

public interface IUsersloginRepository
{
    Task<Userslogin> GetUserByEmailAsync(string email);
    Task<Userslogin> GetUserByIdAsync(int id);
}
