using Microsoft.AspNetCore.Http;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel> GetProfileData(int Id);

    Task UpdateProfileData(ProfileViewModel model , IFormFile ProfileImage);

    Task<bool> UpdatePassword(ChangePasswordViewModel model);
}
