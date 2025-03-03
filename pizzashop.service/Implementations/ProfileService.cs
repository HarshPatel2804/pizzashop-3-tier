using pizzashop.repository.Interfaces;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUsersloginRepository _UsersloginRepository;
    private readonly IUserRepository _UserRepository;

    private readonly IRoleRepository _RoleRepository;
    private readonly ICountryRepository _CountryRepository;

    private readonly IStateRepository _StateRepository;
    private readonly ICityRepository _CityRepository;

    public ProfileService(IUsersloginRepository UsersloginRepository , IUserRepository UserRepository, IRoleRepository RoleRepository,
    ICountryRepository CountryRepository , IStateRepository StateRepository , ICityRepository CityRepository)
    {
        _UsersloginRepository = UsersloginRepository;
        _UserRepository = UserRepository;
        _RoleRepository = RoleRepository;
        _CountryRepository = CountryRepository;
        _StateRepository = StateRepository;
        _CityRepository = CityRepository;
    }
    public async Task<ProfileViewModel> GetProfileData(int Id)
    {
        Console.WriteLine("ID" + Id);
         var userDetails = await _UserRepository.GetUserByIdAsync(Id);
        var user = await _UsersloginRepository.GetUserByIdAsync(Id);

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
            Countries = await _CountryRepository.GetAllCountryAsync(),
            States = await _StateRepository.GetAllStateAsync(userDetails.Countryid),
            Cities = await _CityRepository.GetAllCityAsync(userDetails.Stateid)
        };
        return model;
    }

}
