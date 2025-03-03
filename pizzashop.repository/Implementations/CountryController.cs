using Microsoft.AspNetCore.Mvc.Rendering;
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
         return _context.Countries.Select(r => new SelectListItem
            {
                Value = r.Countryid.ToString(),
                Text = r.Countryname,

            }).ToList();
    }

}
