using Microsoft.AspNetCore.Mvc.Rendering;

namespace pizzashop.repository.Interfaces;

public interface IStateRepository
{
    Task<List<SelectListItem>> GetAllStateAsync(int Countryid);
}
