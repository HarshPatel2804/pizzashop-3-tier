using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class MenuService : IMenuService
{
     private readonly ICategoryRepository _categoryRepository;
     private readonly IItemRepository _itemRepository;

     public MenuService(ICategoryRepository categoryRepository , IItemRepository itemRepository)
    {
       _categoryRepository = categoryRepository;
       _itemRepository = itemRepository;
    }

    public async Task AddCategory(CategoryViewModel model)
    {
        var Category = new Category{
            Categoryname = model.Categoryname,
            Description = model.Description,
        };
        await _categoryRepository.AddCategoryAsync(Category);
    }

    public async Task DeleteCategory(int Categoryid)
    {
        await _categoryRepository.DeleteCategoryAsync(Categoryid);

        await _itemRepository.DeleteItemsByCategory(Categoryid);
    }


    public async Task EditCategory(CategoryViewModel model)
    {
        // Console.WriteLine(model.Description + "Description");
        var category = await _categoryRepository.GetCategoryByIdAsync(model.Categoryid);
        category.Categoryname = model.Categoryname;
        category.Description = model.Description;
        await _categoryRepository.EditCategoryAsync(category);
    }

    public async Task<List<CategoryViewModel>> GetAllCategories()
    {
        return await _categoryRepository.GetAllCategoryAsync();
    }

    public async Task<CategoryViewModel> GetCategoryById(int categoryId)
    {
        var model = await _categoryRepository.GetCategoryByIdAsync(categoryId);

        var viewModel = new CategoryViewModel{
            Categoryid = model.Categoryid,
            Categoryname = model.Categoryname,
            Description = model.Description
        };

        return viewModel;
    }


    public async Task<List<ItemViewModel>> GetItemsByCategory(int CategoryId)
    {
        var model = await _itemRepository.GetItemsByCategoryAsync(CategoryId);

        var itemModel = model.Select(u => new ItemViewModel
        {
            Categoryid = u.Categoryid,
            Itemid = u.Itemid,
            Itemname = u.Itemname,
            Rate = u.Rate,
            Isavailable = u.Isavailable,
            Quantity = u.Quantity,
        }).ToList();
        
        return itemModel;
    }
}
