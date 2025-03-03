using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.Interfaces;

public interface ICityRepository
{
    Task<List<SelectListItem>> GetAllCityAsync(int Stateid);
}
