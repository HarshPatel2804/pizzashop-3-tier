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

        if(pageSize == 0){
            var allItems = await query
            .ToListAsync();
            return (allItems, totalItems);
        }
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

    public async Task<bool> UpdateItemAvailabilityAsync(int itemId, bool isAvailable)
        {
            var item = await _context.Items.FindAsync(itemId);
            
            if (item == null)
                return false;
                
            item.Isavailable = isAvailable;
            item.Modifiedat = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
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
        Modifiers = _context.ModifierGroupModifierMappings
            .Where(mg => mg.ModifierGroupId == img.Modifiergroupid)
            .Select(mg => mg.Modifier)
            .Where(m => m.Isdeleted != true)
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

         public async Task<List<ItemViewModel>> GetMenuItemsbyCategoryAsync(string categoryId, string searchText)
        {
            var query = _context.Items.Where(i => i.Isdeleted != true && i.Isavailable == true).AsQueryable();
            
            if (!string.IsNullOrEmpty(categoryId))
            {
                if (categoryId == "FAV")
                {
                    query = query.Where(i => i.Isfavourite == true);
                }
                else if (categoryId != "ALL")
                {
                    if (int.TryParse(categoryId, out int catId))
                    {
                        query = query.Where(i => i.Categoryid == catId);
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(i => i.Itemname.ToLower().Contains(searchText.ToLower()));
            }
            
            return await query
                .Select(i => new ItemViewModel
                {
                    Itemname = i.Itemname,
                    Rate = i.Rate,
                    Isfavourite = i.Isfavourite,
                    Itemid = i.Itemid,
                    Itemimg = i.Itemimg,
                    Itemtype = i.Itemtype,
                    Isavailable = i.Isavailable
                })
                .ToListAsync();
        }

        public async Task<bool> ToggleFavoriteAsync(int itemId, bool isFavorite)
        {
            try
            {
                var menuItem = await _context.Items.FirstOrDefaultAsync(i => i.Itemid == itemId);
                if (menuItem == null)
                {
                    return false;
                }
                
                menuItem.Isfavourite = isFavorite;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

}
