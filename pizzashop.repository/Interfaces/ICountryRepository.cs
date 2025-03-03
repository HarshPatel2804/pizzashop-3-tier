using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.Interfaces;

public interface ICountryRepository
{
    Task<List<SelectListItem>> GetAllCountryAsync();
}
