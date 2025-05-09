using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class UnitRepository : IUnitRepository
{
    private readonly PizzaShopContext _context;

    public UnitRepository(PizzaShopContext context)
    {
        _context = context;
    }
    public async Task<string> GetUnit(int unitId)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(u => u.Unitid == unitId);
        return unit.Unitname;
    }

    public async Task<List<SelectListItem>> GetUnitsListAsync()
    {
        return _context.Units.OrderBy(u=>u.Unitid).Select(c => new SelectListItem
            {
                Value = c.Unitid.ToString(),
                Text = c.Unitname
            })
            .OrderBy(c => c.Text)
            .ToList();
    }
    }

