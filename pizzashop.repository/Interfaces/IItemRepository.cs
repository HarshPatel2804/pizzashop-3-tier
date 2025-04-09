using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.repository.Interfaces;

public interface IItemRepository
{
    Task<(List<Item> users, int totalItems)> GetItemsByCategoryAsync(int CategoryId , int page, int pageSize, string search);

    Task DeleteItemsByCategory(int Categoryid);

    Task<int> AddItemsAsync(Item model);

    Task DeleteItem(int Itemid);

    Task<Item> GetItemById(int itemId);

    Task EditItemAsync(Item model);

    Task AddItemModifierGroupMappingsAsync(List<Itemmodifiergroupmap> mappings);

    Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId);

    Task RemoveItemModifierGroupMappingsAsync(int itemId);

    Task MassDeleteItem(List<int> Itemid);

    Task<Item> GetItemByName(AddEditItemViewModel model);
}
