using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

namespace pizzashop.repository.Implementations;

public class ModifierRepository : IModifierRepository
{
    private readonly PizzaShopContext _context;

    public ModifierRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task<List<Modifiergroup>> GetAllModifierGroupAsync()
    {
        var model = await _context.Modifiergroups.Where(u => u.Isdeleted != true).OrderBy(u => u.Modifiergroupid).ToListAsync();

        return model;
    }
}

    
