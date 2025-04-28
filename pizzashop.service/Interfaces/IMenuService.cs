using Microsoft.AspNetCore.Http;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetAllCategories();

    Task<List<ModifierGroupViewModel>> GetAllmodifiergroups();

    Task AddCategory(CategoryViewModel model);

    Task<(List<ItemViewModel> itemModel, int totalItems, int totalPages)> GetItemsByCategory(int CategoryId , int page, int pageSize, string search);

    Task<(List<ModifierViewModel> modifiers, int totalModifiers, int totalPages)> GetModifiersByGroup(int ModifierGroupId,int page, int pageSize, string search);

    Task<CategoryViewModel> GetCategoryById(int categoryId);

    Task EditCategory(CategoryViewModel model);

    Task DeleteCategory(int Categoryid);

    Task<bool> UpdateItemAvailabilityAsync(int itemId, bool isAvailable);

    Task DeleteItem(int Itemid);

    Task DeleteModifier(int modifierId , int modifierGroupId);

    Task<AddEditItemViewModel> GetEditItemDetails(int itemId);

    Task<AddEditItemViewModel> GetItemDetails(int categoryId);

    Task AddItemAsync(AddEditItemViewModel addEditItemViewModel , IFormFile ProfileImage);

    Task EditItemAsync(AddEditItemViewModel addEditItemViewModel, IFormFile Itemimg);
    Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId);

    Task<(List<Modifier> modifiers, int totalModifiers, int totalPages)> GetModifierList(int page, int pageSize, string search);

    Task<int> AddModifierGroup(ModifierGroupViewModel model);

    Task<ModifierGroupViewModel> GetSelectedModifiers(int modifierGroupId);

    Task<int> EditModifierGroup(ModifierGroupViewModel model);

    Task<ModifierViewModel> GetModifierGroups();

    Task<int> SaveModifier(ModifierViewModel model);

    Task<ModifierViewModel> GetModifierDetails(int modifierId);

    Task EditModifier(ModifierViewModel model);

    Task DeleteMultipleItems(List<int> itemIds);

    Task DeleteMultipleModifiers(List<int> itemIds , int modifierGroupId);

    Task<Category> GetCategoryByName(CategoryViewModel model);

    Task<Modifiergroup> GetModifierGroupByName(string name, int id);

    Task<Modifier> GetModifierByName(ModifierViewModel model);

    Task<Item> GetItemByName(AddEditItemViewModel model);

    Task<List<ModifierViewModel>> GetModifiersBymodifierGroup(int id);

    Task UpdateCategorySortOrder(List<int> sortOrder);

    Task UpdateModifierGroupSortOrder(List<int> sortOrder);

    Task<int> FirstCategoryId();

    Task DeleteModifiergroup(int modifierGroupId);

    Task<List<ItemViewModel>> GetMenuItemsAsync(string categoryId, string searchText);

    Task<bool> ToggleFavoriteAsync(int itemId, bool isFavorite);
}
