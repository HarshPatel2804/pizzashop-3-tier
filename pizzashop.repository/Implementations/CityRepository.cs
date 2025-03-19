using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class CityRepository : ICityRepository
{
    private readonly PizzaShopContext _context;

    public CityRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<List<SelectListItem>> GetAllCityAsync(int Stateid)
    {
        return await _context.Cities.Where(c => c.Stateid == Stateid).Select(r => new SelectListItem
            {
                Value = r.Cityid.ToString(),
                Text = r.Cityname,

            }).ToListAsync();
    }

}
