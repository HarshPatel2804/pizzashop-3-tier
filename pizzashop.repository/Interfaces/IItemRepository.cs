using pizzashop.repository.Models;

namespace pizzashop.repository.Interfaces;

public interface IItemRepository
{
    Task<(List<Item> users, int totalItems)> GetItemsByCategoryAsync(int CategoryId , int page, int pageSize, string search);

    Task DeleteItemsByCategory(int Categoryid);

    Task AddItemsAsync(Item model);

    Task DeleteItem(int Itemid);

    Task<Item> GetItemById(int itemId);

    Task EditItemAsync(Item model);
}
