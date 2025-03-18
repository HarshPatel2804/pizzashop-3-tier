using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IUnitRepository
{
   Task<string> GetUnit(int unitId);

   Task<List<SelectListItem>> GetUnitsListAsync();
}
