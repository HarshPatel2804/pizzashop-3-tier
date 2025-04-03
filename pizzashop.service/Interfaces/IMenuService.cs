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

    Task<List<ModifierViewModel>> GetModifiersByGroup(int ModifierGroupId);

    Task<CategoryViewModel> GetCategoryById(int categoryId);

    Task EditCategory(CategoryViewModel model);

    Task DeleteCategory(int Categoryid);

    Task DeleteItem(int Itemid);

    Task DeleteModifier(int modifierId);

    Task<AddEditItemViewModel> GetEditItemDetails(int itemId);

    Task<AddEditItemViewModel> GetItemDetails();

    Task AddItemAsync(AddEditItemViewModel addEditItemViewModel , IFormFile ProfileImage);

    Task EditItemAsync(AddEditItemViewModel addEditItemViewModel, IFormFile Itemimg);
    Task<List<ItemModifierGroupMapping>> GetItemModifierGroupsAsync(int itemId);

    Task<(List<Modifier> modifiers, int totalModifiers, int totalPages)> GetModifierList(int page, int pageSize, string search);

    Task<int> AddModifierGroup(ModifierGroupViewModel model);
}
