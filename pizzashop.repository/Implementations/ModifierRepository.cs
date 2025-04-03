using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using Pizzashop.repository.Models;

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

    public async Task<List<ModifierGroupModifierMapping>> GetModifierByGroupAsync(int ModifierGroupId)
    {
        return await _context.ModifierGroupModifierMappings.Where(u => u.ModifierGroupId == ModifierGroupId)
                    .Include(u => u.Modifier)
                    .ToListAsync();
    }

    public async Task DeleteModifier(int modifierId)
    {
        await _context.Modifiers.Where(u => u.Modifierid == modifierId).ForEachAsync(u => u.Isdeleted = true);
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
                    .Include(u => u.Unit)
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

    public async Task<int> AddModifierGroup(Modifiergroup modifierGroup)
    {
        _context.Modifiergroups.Add(modifierGroup);
        _context.SaveChanges();
        return modifierGroup.Modifiergroupid;
    }

    public bool UpdateModifierGroup(Modifiergroup modifierGroup)
    {
        _context.Modifiergroups.Update(modifierGroup);
        return _context.SaveChanges() > 0;
    }

    public async Task AddMappings(ModifierGroupModifierMapping mapping)
        {
            _context.ModifierGroupModifierMappings.Add(mapping);
            _context.SaveChanges();
        }

        public IEnumerable<ModifierGroupModifierMapping> GetByModifierGroupId(int modifierGroupId)
        {
            return _context.ModifierGroupModifierMappings
                .Where(m => m.ModifierGroupId == modifierGroupId)
                .ToList();
        }
}


