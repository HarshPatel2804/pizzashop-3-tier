using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class UsersLoginService : IUsersLoginService
{
     private readonly IUsersloginRepository _UsersloginRepository;

    public UsersLoginService(IUsersloginRepository UsersloginRepository)
    {
        _UsersloginRepository = UsersloginRepository;
    }
    public async Task<Userslogin> GetUserByEmail(string email)
    {
        return await _UsersloginRepository.GetUserByEmailAsync(email);
    }

    public async Task<Userslogin> GetUserById(int id)
    {
        return await _UsersloginRepository.GetUserByIdAsync(id);
    }

}
