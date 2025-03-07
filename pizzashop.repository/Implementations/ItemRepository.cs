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
    public async Task<List<Item>> GetItemsByCategoryAsync(int CategoryId)
    {
        return await _context.Items.Where(u => u.Categoryid == CategoryId).ToListAsync();
    }
}
