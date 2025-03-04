using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Interfaces;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class CountryService : ICountryService
{
    private readonly ICountryRepository _countryRepository;

    public CountryService(ICountryRepository countryRepository)
    {
        _countryRepository = countryRepository;
    }
    public async Task<List<SelectListItem>> GetAllCountry()
    {
        return await _countryRepository.GetAllCountryAsync();
    }

}
