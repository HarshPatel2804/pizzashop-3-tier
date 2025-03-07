using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetAllCategories();

    Task AddCategory(CategoryViewModel model);

    Task<List<ItemViewModel>> GetItemsByCategory(int CategoryId);

    Task<CategoryViewModel> GetCategoryById(int categoryId);

    Task EditCategory(CategoryViewModel model);

    Task DeleteCategory(int Categoryid);
}
