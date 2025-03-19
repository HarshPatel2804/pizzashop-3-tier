using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetAllCategories();

    Task<List<ModifierGroupViewModel>> GetAllmodifiergroups();

    Task AddCategory(CategoryViewModel model);

    Task<List<ItemViewModel>> GetItemsByCategory(int CategoryId);

    Task<List<ModifierViewModel>> GetModifiersByGroup(int ModifierGroupId);

    Task<CategoryViewModel> GetCategoryById(int categoryId);

    Task EditCategory(CategoryViewModel model);

    Task DeleteCategory(int Categoryid);

    Task DeleteItem(int Itemid);

    Task DeleteModifier(int modifierId);

    Task<AddEditItemViewModel> GetItemDetails();

    Task AddItemAsync(AddEditItemViewModel addEditItemViewModel);
}
