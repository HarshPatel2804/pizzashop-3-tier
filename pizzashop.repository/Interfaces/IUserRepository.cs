using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IUserRepository
{
     Task<User> GetUserByIdAsync(int id);
}
