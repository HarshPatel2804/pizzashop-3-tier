using Microsoft.AspNetCore.Mvc.Rendering;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class StateRepository : IStateRepository
{
     private readonly PizzaShopContext _context;

    public StateRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<List<SelectListItem>> GetAllStateAsync(int Countryid)
    {
        return _context.States.Where(c => c.Countryid == Countryid).Select(r => new SelectListItem
            {
                Value = r.Stateid.ToString(),
                Text = r.Statename,

            }).ToList();
    }

}
