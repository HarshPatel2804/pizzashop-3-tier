using System.ComponentModel;
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

    public bool CheckUsername(string Username , int? Id = null){
        return _UsersloginRepository.CheckUsername(Username , Id);
    }

    public bool CheckEmail(string Email, int? Id = null)
        {
            return _UsersloginRepository.CheckEmail(Email,Id);
        }

    public async Task<(List<Userslogin> users, int totalUsers, int totalPages)> GetPaginatedUsersAsync(int page, int pageSize, string search, string sortColumn, string sortOrder)
    {
        var (users, totalUsers) = await _UsersloginRepository.GetPaginatedUsersAsync(page, pageSize, search, sortColumn, sortOrder);

        int totalPages = (int)System.Math.Ceiling((double)totalUsers / pageSize);

        return (users, totalUsers, totalPages);
    }

    public async Task UpdateUserLoginData(UserViewModel model){
        var userloginDetailsById = await _UsersloginRepository.GetUserByIdAsync(model.Id);
        Console.WriteLine(model.status + "status");
        userloginDetailsById.Roleid = model.Role;
        userloginDetailsById.Username = model.Username;
        userloginDetailsById.status = (pizzashop.repository.Models.statustype)Enum.Parse(typeof(pizzashop.repository.Models.statustype), model.status.ToString());

        await _UsersloginRepository.UpdateUserLoginDetails(userloginDetailsById);
    }
}
