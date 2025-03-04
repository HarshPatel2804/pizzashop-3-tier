using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.service.Interfaces;

public interface ICountryService
{
    Task<List<SelectListItem>> GetAllCountry();
}
