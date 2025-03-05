using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class MenuService : IMenuService
{
     private readonly ICategoryRepository _categoryService;

     public MenuService(ICategoryRepository categoryService)
    {
       _categoryService = categoryService;
    }

    public async Task AddCategory(CategoryViewModel model)
    {
        var Category = new Category{
            Categoryname = model.Categoryname,
            Description = model.Description,
        };
        await _categoryService.AddCategoryAsync(Category);
    }


    public async Task<List<CategoryViewModel>> GetAllCategories()
    {
        return await _categoryService.GetAllCategoryAsync();
    }

}
