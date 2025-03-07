using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
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

    public async Task<(List<Userslogin> users, int totalUsers, int totalPages)> GetPaginatedUsersAsync(int page, int pageSize, string search)
    {
        var (users, totalUsers) = await _UsersloginRepository.GetPaginatedUsersAsync(page, pageSize, search);

        int totalPages = (int)System.Math.Ceiling((double)totalUsers / pageSize);

        return (users, totalUsers, totalPages);
    }

    public async Task UpdateUserLoginData(UserViewModel model){
        var userloginDetailsById = await _UsersloginRepository.GetUserByIdAsync(model.Id);

        userloginDetailsById.Roleid = model.Role;
        userloginDetailsById.Username = model.Username;
        userloginDetailsById.status = (repository.Models.statustype)model.status;

        await _UsersloginRepository.UpdateUserLoginDetails(userloginDetailsById);
    }
}
