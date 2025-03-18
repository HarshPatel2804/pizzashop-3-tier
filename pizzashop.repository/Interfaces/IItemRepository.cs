using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IItemRepository
{
    Task<List<Item>> GetItemsByCategoryAsync(int CategoryId);

    Task DeleteItemsByCategory(int Categoryid);

    Task AddItemsAsync(Item model);

    Task DeleteItem(int Itemid);
}
