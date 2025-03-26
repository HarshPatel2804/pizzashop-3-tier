using Microsoft.AspNetCore.Mvc.Rendering;
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

    public async Task<List<Modifier>> GetModifierByGroupAsync(int ModifierGroupId)
    {
        return await _context.Modifiers.Where(u => u.Modifiergroupid == ModifierGroupId && u.Isdeleted != true).ToListAsync();
    }

    public async Task DeleteModifier(int modifierId)
    {
        await _context.Modifiers.Where(u => u.Modifierid== modifierId).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> GetAllmodifierGroups()
    {
        return await _context.Modifiergroups.Select(r => new SelectListItem
            {
                Value = r.Modifiergroupid.ToString(),
                Text = r.Modifiergroupname,

            }).ToListAsync();
    }

    public async Task<(List<Modifier> modifiers, int totalModifiers)> GetAllModifierAsync(int page, int pageSize, string search)
    {

        var query = _context.Modifiers
                    .Where(u => u.Isdeleted == false)
                    .OrderBy(u => u.Modifierid)
                    .Where(u => string.IsNullOrEmpty(search) ||
                        u.Modifiername.ToLower().Contains(search.ToLower()));

        int totalModifiers = await query.CountAsync();

        var modifiers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (modifiers, totalModifiers);
    }
}

    
