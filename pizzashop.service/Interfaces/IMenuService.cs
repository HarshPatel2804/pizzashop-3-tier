using pizzashop.repository.ViewModels;

namespace pizzashop.service.Interfaces;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetAllCategories();

    Task AddCategory(CategoryViewModel model);
}
