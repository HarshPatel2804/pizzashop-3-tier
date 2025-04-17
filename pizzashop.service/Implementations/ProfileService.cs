using Microsoft.AspNetCore.Http;
using pizzashop.repository.Interfaces;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;
using pizzashop.service.Utils;

namespace pizzashop.service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUsersloginRepository _UsersloginRepository;
    private readonly IUserRepository _UserRepository;

    private readonly IRoleRepository _RoleRepository;
    private readonly ICountryRepository _CountryRepository;

    private readonly IStateRepository _StateRepository;
    private readonly ICityRepository _CityRepository;

    private readonly IImageService _imageService;

    public ProfileService(IUsersloginRepository UsersloginRepository , IUserRepository UserRepository, IRoleRepository RoleRepository,
    ICountryRepository CountryRepository , IStateRepository StateRepository , ICityRepository CityRepository , IImageService imageService)
    {
        _UsersloginRepository = UsersloginRepository;
        _UserRepository = UserRepository;
        _RoleRepository = RoleRepository;
        _CountryRepository = CountryRepository;
        _StateRepository = StateRepository;
        _CityRepository = CityRepository;
        _imageService = imageService;
    }
    public async Task<ProfileViewModel> GetProfileData(int Id)
    {
        Console.WriteLine("ID" + Id);
         var userDetails = await _UserRepository.GetUserByIdAsync(Id);
        var user = await _UsersloginRepository.GetUserByIdAsync(Id);
         var role = await _RoleRepository.GetUserRoleAsync(user.Roleid);

        var model = new ProfileViewModel{
            Id = userDetails.Userid,
            FirstName = userDetails.Firstname,
            LastName = userDetails.Lastname,
            Phone = userDetails.Phone,
            Username = user.Username,
            Email = user.Email,
            Address = userDetails.Address,
            Zipcode = userDetails.Zipcode,
            CountryId = userDetails.Countryid,
            StateId = userDetails.Stateid,
            CityId = userDetails.Cityid,
            Profileimg = userDetails.Profileimg,
            Rolename = role.Rolename,
            Countries = await _CountryRepository.GetAllCountryAsync(),
            States = await _StateRepository.GetAllStateAsync(userDetails.Countryid),
            Cities = await _CityRepository.GetAllCityAsync(userDetails.Stateid)
        };
        return model;
    }

     public async Task UpdateProfileData(ProfileViewModel model , IFormFile ProfileImage)
     {
         var userDetails = await _UserRepository.GetUserByIdAsync(model.Id);
         var user = await _UsersloginRepository.GetUserByIdAsync(model.Id);
        

         userDetails.Firstname = model.FirstName;
         userDetails.Lastname = model.LastName;
         userDetails.Phone = model.Phone;
         userDetails.Address = model.Address;
         userDetails.Zipcode = model.Zipcode;
         userDetails.Countryid = model.CountryId;
         userDetails.Stateid = model.StateId;
         userDetails.Cityid = model.CityId;
         user.Username = model.Username;

        if(userDetails.Profileimg != null){
         string fullPath = Path.Combine("wwwroot", "images", "uploads", userDetails.Profileimg);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting previous profile image: {ex.Message}");
                }
            }
        }
         if(ProfileImage != null)
         {
         userDetails.Profileimg = await _imageService.GiveImagePath(ProfileImage);
         }

         await _UserRepository.UpdateUser(userDetails);
         await _UsersloginRepository.UpdateUserLoginDetails(user);
     }

     public async Task<bool> UpdatePassword(ChangePasswordViewModel model){
        var usersLogin = await _UsersloginRepository.GetUserByEmailAsync(model.Email);
        if (usersLogin == null || !PasswordUtills.VerifyPassword(model.OldPassword, usersLogin.Passwordhash))
        {
            Console.WriteLine(model.Email + model.OldPassword);
            return false;
        }
        else 
        {
            usersLogin.Passwordhash = model.Password;
             await _UsersloginRepository.UpdateUserLoginDetails(usersLogin);
            return true;
        }
     }

}
