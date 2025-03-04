using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel> GetProfileData(int Id);

    Task UpdateProfileData(ProfileViewModel model);

    Task<bool> UpdatePassword(ChangePasswordViewModel model);
}
