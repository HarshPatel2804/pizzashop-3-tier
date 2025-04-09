using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Implementations;

public class ItemRepository : IItemRepository
{
    private readonly PizzaShopContext _context;

    public ItemRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public async Task DeleteItemsByCategory(int Categoryid)
    {      
        var itemIds = await _context.Items
        .Where(u => u.Categoryid == Categoryid && u.Isdeleted == false)
        .Select(u => u.Itemid)
        .ToListAsync();

        await MassDeleteItem(itemIds);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItem(int Itemid)
    {
        await _context.Items.Where(u => u.Itemid == Itemid).ForEachAsync(u => u.Isdeleted = true);
        var mappings = await _context.Itemmodifiergroupmaps.Where(m => m.Itemid == Itemid).ToListAsync();
        _context.Itemmodifiergroupmaps.RemoveRange(mappings);
        await _context.SaveChangesAsync();
    }

    public async Task MassDeleteItem(List<int> Itemid)
    {
        await _context.Items.Where(u => Itemid.Contains(u.Itemid)).ForEachAsync(u => u.Isdeleted = true);
        var mappings = await _context.Itemmodifiergroupmaps.Where(m => Itemid.Contains(m.Itemid)).ToListAsync();
        _context.Itemmodifiergroupmaps.RemoveRange(mappings);
        await _context.SaveChangesAsync();

    }

    public async Task<(List<Item> users, int totalItems)> GetItemsByCategoryAsync(int CategoryId , int page, int pageSize, string search)
    {
        var query = _context.Items
        .Where(u => u.Categoryid == CategoryId && u.Isdeleted != true)
        .Where(u => string.IsNullOrEmpty(search) ||
                        u.Itemname.ToLower().Contains(search.ToLower()));

        int totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalItems);
    }

    public async Task<Item> GetItemById(int itemId)
    {
        return await _context.Items.FirstOrDefaultAsync(u => u.Itemid == itemId);
    }

     public async Task<int> AddItemsAsync(Item model)
    {
        await _context.Items.AddAsync(model);
        await _context.SaveChangesAsync();

       return model.Itemid;
    }

    public async Task EditItemAsync(Item model)
    {
        _context.Items.Update(model);
        await _context.SaveChangesAsync();
    }
    public async Task AddItemModifierGroupMappingsAsync(List<Itemmodifiergroupmap> mappings)
    {
        await _context.Itemmodifiergroupmaps.AddRangeAsync(mappings);
        await _context.SaveChangesAsync();
    }

     public async Task RemoveItemModifierGroupMappingsAsync(int itemId)
    {
        var existingMappings = await _context.Itemmodifiergroupmaps
            .Where(img => img.Itemid == itemId)
            .ToListAsync();

        _context.Itemmodifiergroupmaps.RemoveRange(existingMappings);
        await _context.SaveChangesAsync();
    }

     public async Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId)
    {
            var itemModifierGroups = await _context.Itemmodifiergroupmaps
                .Where(img => img.Itemid == itemId)
                .Select(img => new ItemModifierGroupMapping
                {
                    Itemmodifiergroupid = img.Itemmodifiergroupid,
                    Itemid = img.Itemid,
                    Modifiergroupid = img.Modifiergroupid,
                    ModifiergroupName = img.Modifiergroup.Modifiergroupname,
                    Minselectionrequired = img.Minselectionrequired,
                    Maxselectionallowed = img.Maxselectionallowed,
                    Modifiers = _context.Modifiers
                        .Where(m => m.Modifiergroupid == img.Modifiergroupid && m.Isdeleted != true)
                        .Select(m => new Modifier
                        {
                            Modifierid = m.Modifierid,
                            Modifiername = m.Modifiername,
                            Modifiergroupid = m.Modifiergroupid,
                            Rate = m.Rate,
                            Quantity = m.Quantity,
                            Unitid = m.Unitid,
                            Description = m.Description
                        })
                        .ToList()
                })
                .ToListAsync();

            return itemModifierGroups;
        
    }

    public async Task<Item> GetItemByName(AddEditItemViewModel model)
        {
            return await _context.Items
                .FirstOrDefaultAsync(mg => 
                    mg.Itemname.ToLower() == model.Itemname.ToLower() && 
                    mg.Itemid != model.Itemid && 
                    mg.Isdeleted != true);
        }

}
