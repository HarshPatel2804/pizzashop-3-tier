using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
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
        var model = await _context.Modifiergroups.Where(u => u.Isdeleted != true).OrderBy(u => u.SortOrder).ToListAsync();

        return model;
    }

    public async Task<Modifiergroup> GetModifierGroupByIdAsync(int modifierGroupId)
    {
        return await _context.Modifiergroups.FirstOrDefaultAsync(u => u.Modifiergroupid == modifierGroupId);

    }

    public async Task UpdateSortOrderOfModifierGroup(List<int> sortOrder)
    {

        for (int i = 0; i < sortOrder.Count; i++)
        {
            Modifiergroup modifiergroup = _context.Modifiergroups.FirstOrDefault(s => s.Modifiergroupid == sortOrder[i]);

            if (modifiergroup != null)
            {
                modifiergroup.SortOrder = i + 1;
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<(List<ModifierGroupModifierMapping>, int totalModifiers)> GetModifierByGroupAsync(int ModifierGroupId, int page, int pageSize, string search)
    {
        var query = _context.ModifierGroupModifierMappings.Where(u => u.ModifierGroupId == ModifierGroupId)
                    .Include(u => u.Modifier)
                    .Where(u => u.Modifier.Isdeleted == false)
                    .OrderBy(u => u.Modifier.Modifierid)
                    .Where(u => string.IsNullOrEmpty(search) ||
                        u.Modifier.Modifiername.ToLower().Contains(search.ToLower()) ||
                        u.Modifier.Rate.ToString().Contains(search.ToLower()));

        int totalModifiers = await query.CountAsync();

        if (pageSize == 0)
        {
            var allModifiers = await query
            .ToListAsync();
            return (allModifiers, totalModifiers);
        }
        var modifiers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (modifiers, totalModifiers);
    }

    public async Task DeleteModifier(int modifierId, int modifierGroupId)
    {
        var mappings = await _context.ModifierGroupModifierMappings.Where(u => u.ModifierGroupId == modifierGroupId && u.ModifierId == modifierId)
                    .Include(u => u.Modifier)
                    .ToListAsync();
        _context.ModifierGroupModifierMappings.RemoveRange(mappings);
        await _context.SaveChangesAsync();
        var count = await _context.ModifierGroupModifierMappings.Where(u => u.ModifierId == modifierId).CountAsync();
        if (count == 0)
        {
            await _context.Modifiers.Where(u => u.Modifierid == modifierId).ForEachAsync(u => u.Isdeleted = true);
        }
        await _context.SaveChangesAsync();
    }
    public async Task DeleteModifierGroup(int modifierId, int modifierGroupId)
    {
        var mappings = await _context.ModifierGroupModifierMappings.Where(u => u.ModifierGroupId == modifierGroupId && u.ModifierId == modifierId)
                    .Include(u => u.Modifier)
                    .ToListAsync();
        _context.ModifierGroupModifierMappings.RemoveRange(mappings);
        await _context.SaveChangesAsync();
        var count = await _context.ModifierGroupModifierMappings.Where(u => u.ModifierId == modifierId).CountAsync();
        if (count == 0)
        {
            await _context.Modifiers.Where(u => u.Modifierid == modifierId).ForEachAsync(u => u.Isdeleted = true);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<List<SelectListItem>> GetAllmodifierGroups()
    {
        return await _context.Modifiergroups.Select(r => new SelectListItem
        {
            Value = r.Modifiergroupid.ToString(),
            Text = r.Modifiergroupname,

        })
        .OrderBy(c => c.Text)
        .ToListAsync();
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

        if (pageSize == 0)
        {
            var allModifiers = await query
            .ToListAsync();

            return (allModifiers, totalModifiers);
        }

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
        var modifier = _context.Modifiergroups.FirstOrDefault(m => m.Modifiergroupid == modifierGroup.Modifiergroupid);
        modifier.Modifiergroupname = modifierGroup.Modifiergroupname;
        modifier.Description = modifierGroup.Description;
        return _context.SaveChanges() > 0;
    }

    public async Task AddMappings(ModifierGroupModifierMapping mapping)
    {
        _context.ModifierGroupModifierMappings.Add(mapping);
        _context.SaveChanges();
    }
    public async Task DeleteMappings(int modifierGroupId)
    {
        var mappings = await _context.ModifierGroupModifierMappings.Where(m => m.ModifierGroupId == modifierGroupId).ToListAsync();
        _context.ModifierGroupModifierMappings.RemoveRange(mappings);
        _context.SaveChanges();
    }
    public async Task RemoveMappings(int modifierId)
    {
        var mappings = await _context.ModifierGroupModifierMappings.Where(m => m.ModifierId == modifierId).ToListAsync();
        _context.ModifierGroupModifierMappings.RemoveRange(mappings);
        _context.SaveChanges();
    }

    public IEnumerable<ModifierGroupModifierMapping> GetByModifierGroupId(int modifierGroupId)
    {
        return _context.ModifierGroupModifierMappings
            .Where(m => m.ModifierGroupId == modifierGroupId)
            .ToList();
    }

    public async Task<Modifier> GetModifierByIdAsync(int modifierId)
    {
        return await _context.Modifiers.FirstOrDefaultAsync(m => m.Modifierid == modifierId);
    }

    public async Task<int> AddModifier(Modifier modifier)
    {
        await _context.Modifiers.AddAsync(modifier);
        await _context.SaveChangesAsync();
        return modifier.Modifierid;
    }
    public async Task UpdateModifier(Modifier modifier)
    {
        _context.Modifiers.Update(modifier);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<ModifierGroupModifierMapping> GetMappingsByModifierId(int modifierId)
    {
        return _context.ModifierGroupModifierMappings
            .Where(m => m.ModifierId == modifierId)
            .ToList();
    }

    public async Task<Modifiergroup> GetModifierGroupByName(string name, int id)
    {
        return await _context.Modifiergroups
            .FirstOrDefaultAsync(mg =>
                mg.Modifiergroupname.ToLower() == name.ToLower() &&
                mg.Modifiergroupid != id &&
                mg.Isdeleted != true);
    }
    public async Task<Modifier> GetModifierByName(ModifierViewModel model)
    {
        return await _context.Modifiers
            .FirstOrDefaultAsync(mg =>
                mg.Modifiername.ToLower() == model.Modifiername.ToLower() &&
                mg.Modifierid != model.Modifierid &&
                mg.Isdeleted != true);
    }

    public async Task<List<Modifier>> GetModifiersBymodifierGroup(int id)
    {
        var modifiers = await _context.ModifierGroupModifierMappings
            .Where(mg => mg.ModifierGroupId == id)
            .Select(mg => mg.Modifier)
            .Where(m => m.Isdeleted == false)
            .ToListAsync();

        return modifiers;
    }

    public async Task DeleteModifierGroupAsync(int modifierGroupId)
    {
        var modifierGroup = await _context.Modifiergroups.FirstOrDefaultAsync(m => m.Modifiergroupid == modifierGroupId);
        modifierGroup.Isdeleted = true;
        await _context.SaveChangesAsync();
    }
}


