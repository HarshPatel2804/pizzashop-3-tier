using pizzashop.repository.Models;

namespace pizzashop.service.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(int id);
}
