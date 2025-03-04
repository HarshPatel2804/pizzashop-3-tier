using Microsoft.AspNetCore.Mvc;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class UserService : IUserService
{
    
     private readonly IUserRepository _UserRepository;
     private readonly IUsersloginRepository _usersloginRepository;

    public UserService(IUserRepository UserRepository, IUsersloginRepository usersloginRepository)
    {
        _UserRepository = UserRepository;
        _usersloginRepository = usersloginRepository;
    }
    public async Task<User> GetUserById(int id)
    {
        return await _UserRepository.GetUserByIdAsync(id);
    }

    public async Task DeleteUser(int id){
       var user = await _UserRepository.GetUserByIdAsync(id);
       user.Isdeleted = true;
       await _UserRepository.UpdateUser(user);
    }

    
    public async Task AddUser(UserViewModel model){
        var user = await _UserRepository.AddUser(model);

        await _usersloginRepository.AddUserloginDetails(model , user.Userid);
    }
}
