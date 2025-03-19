using Microsoft.EntityFrameworkCore;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;

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
        // Console.WriteLine(Categoryid + "items");
        await _context.Items.Where(u => u.Categoryid== Categoryid).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItem(int Itemid)
    {
        // Console.WriteLine(Categoryid + "items");
        await _context.Items.Where(u => u.Itemid== Itemid).ForEachAsync(u => u.Isdeleted = true);
        await _context.SaveChangesAsync();
    }


    public async Task<List<Item>> GetItemsByCategoryAsync(int CategoryId)
    {
        return await _context.Items.Where(u => u.Categoryid == CategoryId && u.Isdeleted != true).ToListAsync();
    }

    public async Task<Item> GetItemById(int itemId)
    {
        return await _context.Items.FirstOrDefaultAsync(u => u.Itemid == itemId);
    }

     public async Task AddItemsAsync(Item model)
    {
        await _context.Items.AddAsync(model);
        await _context.SaveChangesAsync();
    }

    public async Task EditItemAsync(Item model)
    {
        _context.Items.Update(model);
        await _context.SaveChangesAsync();
    }

}
