using Microsoft.AspNetCore.Http;
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

     private readonly IUsersLoginService _usersLoginService;

     private readonly IImageService  _imageService;

    public UserService(IUserRepository UserRepository, IUsersloginRepository usersloginRepository,IUsersLoginService usersLoginService,IImageService  imageService)
    {
        _UserRepository = UserRepository;
        _usersloginRepository = usersloginRepository;
        _usersLoginService = usersLoginService;
        _imageService = imageService;
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

    
    public async Task AddUser(UserViewModel model , IFormFile ProfileImg){
        if(ProfileImg != null)
         {
         model.Profileimg = await _imageService.GiveImagePath(ProfileImg);
         Console.WriteLine(model.Profileimg);
         }
         else{
            Console.WriteLine("empty");
         }

        var user = await _UserRepository.AddUser(model);
        await _usersloginRepository.AddUserloginDetails(model , user.Userid);
    }

    public async Task<UserViewModel> GetUserData(int id){
        return await _UserRepository.GetUserDataAsync(id);
    }

    public bool CheckPhone(string Phone, int? Id = null)
        {
            return _UserRepository.CheckPhone(Phone,Id);
        }

        

    public async Task UpdateUserData(UserViewModel model , IFormFile ProfileImg)
     {
         var userDetails = await _UserRepository.GetUserByIdAsync(model.Id);
        if(ProfileImg != null)
         {
         userDetails.Profileimg =await  _imageService.GiveImagePath(ProfileImg);
         Console.WriteLine(model.Profileimg);
         }
         userDetails.Firstname = model.FirstName;
         userDetails.Lastname = model.LastName;
         userDetails.Phone = model.Phone;
         userDetails.Address = model.Address;
         userDetails.Zipcode = model.Zipcode;
         userDetails.Countryid = model.CountryId;
         userDetails.Stateid = model.StateId;
         userDetails.Cityid = model.CityId;
        
         await _UserRepository.UpdateUser(userDetails);
         await _usersLoginService.UpdateUserLoginData(model);
     }

}
