using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class CountryRepository : ICountryRepository
{
     private readonly PizzaShopContext _context;

    public CountryRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<List<SelectListItem>> GetAllCountryAsync()
    {
         return await _context.Countries.Select(r => new SelectListItem
            {
                Value = r.Countryid.ToString(),
                Text = r.Countryname,

            })
            .OrderBy(c => c.Text)
            .ToListAsync();
    }

}
