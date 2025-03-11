using Microsoft.AspNetCore.Authorization;
using pizzashop.repository.Interfaces;
using pizzashop.repository.Models;
using pizzashop.repository.ViewModels;
using pizzashop.service.Interfaces;

namespace pizzashop.service.Implementations;

public class MenuService : IMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IItemRepository _itemRepository;

    private readonly IModifierRepository _modifierRepository;

    private readonly IUnitRepository _unitRepository;

    public MenuService(ICategoryRepository categoryRepository, IItemRepository itemRepository, IModifierRepository modifierRepository, IUnitRepository unitRepository)
    {
        _categoryRepository = categoryRepository;
        _itemRepository = itemRepository;
        _modifierRepository = modifierRepository;
        _unitRepository = unitRepository;
    }

    public async Task AddCategory(CategoryViewModel model)
    {
        var Category = new Category
        {
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

    public async Task<List<ModifierGroupViewModel>> GetAllmodifiergroups()
    {
        var model = await _modifierRepository.GetAllModifierGroupAsync(); ;
        var viewModel = model.Select(u => new ModifierGroupViewModel
        {
            Modifiergroupid = u.Modifiergroupid,
            Modifiergroupname = u.Modifiergroupname,
            Description = u.Description,

        }).ToList();

        return viewModel;
    }


    public async Task<CategoryViewModel> GetCategoryById(int categoryId)
    {
        var model = await _categoryRepository.GetCategoryByIdAsync(categoryId);

        var viewModel = new CategoryViewModel
        {
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

    public async Task<List<ModifierViewModel>> GetModifiersByGroup(int ModifierGroupId)
    {
        var model = await _modifierRepository.GetModifierByGroupAsync(ModifierGroupId);

        var modifierModel = new List<ModifierViewModel>();

        foreach (var u in model)
        {
            var unitName = await _unitRepository.GetUnit(u.Unitid);

            modifierModel.Add(new ModifierViewModel
            {
                Modifiergroupid = u.Modifiergroupid,
                Modifierid = u.Modifierid,
                Modifiername = u.Modifiername,
                Rate = u.Rate,
                Quantity = u.Quantity,
                UnitName = unitName
            });
        }

        return modifierModel;
    }

}
