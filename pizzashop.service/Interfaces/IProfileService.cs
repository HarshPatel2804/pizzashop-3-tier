using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel> GetProfileData(int Id);
}
