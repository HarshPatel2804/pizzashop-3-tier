using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class UserService : IUserService
{
    
     private readonly IUserRepository _UserRepository;

    public UserService(IUserRepository UserRepository)
    {
        _UserRepository = UserRepository;
    }
    public async Task<User> GetUserById(int id)
    {
        return await _UserRepository.GetUserByIdAsync(id);
    }

}
